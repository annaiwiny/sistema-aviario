import React, { useState, useEffect } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, KeyboardAvoidingView, Platform, Alert, ActivityIndicator } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal';

interface LineageItem {
    id?: number;
    race: string;
    quantity: string;
}

export default function EditBatchScreen() {
    const { id } = useLocalSearchParams();
    const router = useRouter();

    // States
    const [accommodationDate, setAccommodationDate] = useState('');
    const [lineages, setLineages] = useState<LineageItem[]>([]);
    
    // Loading states
    const [isLoading, setIsLoading] = useState(false);
    const [isSaving, setIsSaving] = useState(false);
    
    // Modal state
    const [showSuccessModal, setShowSuccessModal] = useState(false);

    useEffect(() => {
        if (id) loadLotData();
    }, [id]);

    const formatDateToDisplay = (isoDate: string) => {
        if (!isoDate) return '';
        const date = new Date(isoDate);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        return `${day}/${month}/${year}`;
    };

    const formatDateToISO = (displayDate: string) => {
        // Converte 20/05/2026 -> 2026-05-20T00:00:00Z
        const parts = displayDate.split('/');
        if (parts.length === 3) {
            return `${parts[2]}-${parts[1]}-${parts[0]}T00:00:00Z`;
        }
        return null;
    };

    const loadLotData = async () => {
        try {
            setIsLoading(true);
            const token = await AsyncStorage.getItem('userToken');

            const response = await fetch(`${API_URL}/api/Lot/${id}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const data = await response.json();
                setAccommodationDate(formatDateToDisplay(data.accommodationDate));
                
                // Mapeia as linhagens vindas da API
                if (data.lineages && Array.isArray(data.lineages)) {
                    setLineages(data.lineages.map((l: any) => ({
                        id: l.id,
                        race: l.race,
                        quantity: String(l.quantity)
                    })));
                }
            } else {
                Alert.alert('Erro', 'Não foi possível carregar os dados do lote.');
            }
        } catch (error) {
            console.error('Erro ao carregar dados', error);
        } finally {
            setIsLoading(false);
        }
    };

    // Manipulação da Data (Máscara simples)
    const handleDateChange = (text: string) => {
        let formatted = text.replace(/\D/g, '');
        if (formatted.length > 2) formatted = formatted.replace(/^(\d{2})(\d)/, '$1/$2');
        if (formatted.length > 5) formatted = formatted.replace(/^(\d{2})\/(\d{2})(\d)/, '$1/$2/$3');
        if (formatted.length > 10) formatted = formatted.slice(0, 10);
        setAccommodationDate(formatted);
    };

    // Manipulação das Linhagens
    const updateLineage = (index: number, field: keyof LineageItem, value: string) => {
        const updated = [...lineages];
        updated[index] = { ...updated[index], [field]: value };
        setLineages(updated);
    };

    const addLineage = () => {
        setLineages([...lineages, { race: '', quantity: '' }]);
    };

    const handleUpdate = async () => {
        try {
            setIsSaving(true);
            const token = await AsyncStorage.getItem('userToken');

            if (!accommodationDate) {
                Alert.alert("Erro", "Preencha a data de alojamento");
                setIsSaving(false);
                return;
            }

            const isoDate = formatDateToISO(accommodationDate);
            if (!isoDate) {
                Alert.alert("Erro", "Data inválida");
                setIsSaving(false);
                return;
            }

            const payload = {
                accommodationDate: isoDate,
                lineages: lineages.map(l => ({
                    // Se o item tiver ID, manda ele. Se for novo, o backend deve tratar (ou ignorar se for só update)
                    // Pelo swagger parece que ele aceita a lista completa para substituir/atualizar
                    race: l.race,
                    quantity: parseInt(l.quantity) || 0 
                }))
            };

            // CORREÇÃO AQUI: Mudado de PUT para PATCH
            const response = await fetch(`${API_URL}/api/Lot/${id}`, {
                method: 'PATCH', 
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                setShowSuccessModal(true);
            } else {
                const errorData = await response.json();
                // Tenta pegar a mensagem de erro detalhada do backend ou usa uma genérica
                const msg = errorData.message || errorData.title || 'Falha ao atualizar lote.';
                Alert.alert('Erro', msg);
            }

        } catch (error) {
            console.error('Erro ao salvar', error);
            Alert.alert('Erro', 'Falha na conexão com o servidor.');
        } finally {
            setIsSaving(false);
        }
    };

    const handleSuccessClose = () => {
        setShowSuccessModal(false);
        router.back(); 
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <KeyboardAvoidingView
                behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
                style={{ flex: 1 }}
            >
                <ScrollView contentContainerStyle={{ flexGrow: 1, padding: 24, paddingTop: 24 }}>

                    {/* Header (Igual Imagem 01) */}
                    <TouchableOpacity
                        className="flex-row items-center z-10 mb-6 self-start"
                        onPress={() => router.back()}
                    >
                        <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                        <Text className="text-[#8B5CF6] font-bold text-base ml-1">voltar</Text>
                    </TouchableOpacity>

                    {/* Titulo */}
                    <Text className="text-3xl font-bold text-black mb-8">
                        Editar Lote {String(id).padStart(2, '0')}
                    </Text>

                    {isLoading ? (
                        <ActivityIndicator size="large" color="#8B5CF6" className="mt-10" />
                    ) : (
                        <View className="space-y-6">
                            
                            {/* ALOJAMENTO */}
                            <View>
                                <Text className="text-[#9CA3AF] font-bold mb-2 uppercase text-sm">ALOJAMENTO</Text>
                                <Text className="text-black font-bold mb-1">Data</Text>
                                <TextInput
                                    className="bg-gray-200 rounded-lg p-4 text-base text-gray-800"
                                    value={accommodationDate}
                                    onChangeText={handleDateChange}
                                    placeholder="DD/MM/AAAA"
                                    keyboardType="numeric"
                                    maxLength={10}
                                />
                            </View>

                            {/* LINHAGENS DINÂMICAS */}
                            {lineages.map((item, index) => (
                                <View key={index} className="mt-2">
                                    <Text className="text-[#9CA3AF] font-bold mb-2 uppercase text-sm">
                                        LINHAGEM {String(index + 1).padStart(2, '0')}
                                    </Text>

                                    <View className="mb-4">
                                        <Text className="text-black font-bold mb-1">Nome da raça</Text>
                                        <TextInput
                                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800"
                                            value={item.race}
                                            onChangeText={(text) => updateLineage(index, 'race', text)}
                                        />
                                    </View>

                                    <View>
                                        <Text className="text-black font-bold mb-1">Quantidade</Text>
                                        <TextInput
                                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800"
                                            value={item.quantity}
                                            onChangeText={(text) => updateLineage(index, 'quantity', text)}
                                            keyboardType="numeric"
                                        />
                                    </View>
                                </View>
                            ))}

                            {/* Botão Adicionar Mais (Igual Imagem 01) */}
                            <TouchableOpacity
                                onPress={addLineage}
                                className="flex-row items-center mt-2 mb-8"
                            >
                                <Text className="text-[#9CA3AF] font-bold mr-2 uppercase text-sm">
                                    CADASTRAR MAIS UMA LINHAGEM
                                </Text>
                                <Ionicons name="add" size={24} color="#9CA3AF" />
                            </TouchableOpacity>

                            {/* Botão Atualizar */}
                            <View className="items-center pb-8">
                                <TouchableOpacity
                                    className="bg-[#8B5CF6] w-full py-4 rounded-2xl items-center shadow-lg shadow-purple-200"
                                    onPress={handleUpdate}
                                    disabled={isSaving}
                                >
                                    {isSaving ? (
                                        <ActivityIndicator color="white" />
                                    ) : (
                                        <Text className="text-white font-bold text-lg">Atualizar</Text>
                                    )}
                                </TouchableOpacity>
                            </View>

                        </View>
                    )}

                </ScrollView>
            </KeyboardAvoidingView>

            {/* Modal de Sucesso */}
            <SuccessModal 
                visible={showSuccessModal} 
                onClose={handleSuccessClose} 
                message="LOTE ATUALIZADO COM SUCESSO"
            />
        </SafeAreaView>
    );
}