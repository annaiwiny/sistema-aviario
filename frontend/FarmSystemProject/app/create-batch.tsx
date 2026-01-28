import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, Alert, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import SuccessModal from '@/components/SuccessModal';
import { API_URL } from '@/constants/Api';
import AsyncStorage from '@react-native-async-storage/async-storage';

// Interface para os itens de linhagem
interface LineageItem {
    id: number;
    raceName: string;
    quantity: string;
}

export default function CreateBatch() {
    const router = useRouter();
    const [accommodationDate, setAccommodationDate] = useState('');

    // Lista dinâmica de linhagens (começa com uma)
    const [lineages, setLineages] = useState<LineageItem[]>([
        { id: 1, raceName: '', quantity: '' }
    ]);

    const [isLoading, setIsLoading] = useState(false);
    const [showSuccess, setShowSuccess] = useState(false);

    // Adicionar nova linhagem aos campos
    const addLineage = () => {
        const newItem: LineageItem = {
            id: Date.now(),
            raceName: '',
            quantity: ''
        };
        setLineages([...lineages, newItem]);
    };

    // Atualizar valores dos inputs dinâmicos
    const updateLineage = (id: number, field: keyof LineageItem, value: string) => {
        const updated = lineages.map(item => {
            if (item.id === id) {
                return { ...item, [field]: value };
            }
            return item;
        });
        setLineages(updated);
    };

    const handleRegister = async () => {
        // Validação simples
        if (!accommodationDate) {
            Alert.alert('Erro', 'Informe a data de alojamento.');
            return;
        }

        for (const item of lineages) {
            if (!item.raceName || !item.quantity) {
                Alert.alert('Erro', 'Preencha todos os campos da linhagem.');
                return;
            }
        }

        setIsLoading(true);

        try {
            const token = await AsyncStorage.getItem('userToken');
            const aviaryName = await AsyncStorage.getItem('aviaryName');

            if (!token) {
                Alert.alert('Erro', 'Usuário não autenticado.');
                router.replace('/');
                return;
            }

            // Converter data DD/MM/AAAA para ISO
            const dateParts = accommodationDate.split('/');
            const dateISO = `${dateParts[2]}-${dateParts[1]}-${dateParts[0]}T00:00:00Z`;

            const payload = {
                accommodationDate: dateISO,
                items: lineages.map(l => ({
                    race: l.raceName,
                    quantity: parseInt(l.quantity)
                }))
            };

            const response = await fetch(`${API_URL}/api/Lot`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                setShowSuccess(true);
            } else {
                const errorData = await response.json();
                Alert.alert('Erro', errorData.title || 'Falha ao cadastrar lote.');
            }

        } catch (error) {
            console.error(error);
            Alert.alert('Erro', 'Falha na conexão com o servidor.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleSuccessClose = () => {
        setShowSuccess(false);
        router.back();
    };

    // Formatar data (simples)
    const handleDateChange = (text: string) => {
        // Máscara simples DD/MM/AAAA
        let formatted = text.replace(/\D/g, '');
        if (formatted.length > 2) formatted = formatted.replace(/^(\d{2})(\d)/, '$1/$2');
        if (formatted.length > 5) formatted = formatted.replace(/^(\d{2})\/(\d{2})(\d)/, '$1/$2/$3');
        if (formatted.length > 10) formatted = formatted.slice(0, 10);
        setAccommodationDate(formatted);
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <ScrollView
                contentContainerStyle={{ flexGrow: 1, padding: 24 }}
                showsVerticalScrollIndicator={false}
            >
                {/* Header */}
                <TouchableOpacity
                    onPress={() => router.back()}
                    className="flex-row items-center mb-6 self-start"
                >
                    <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                    <Text className="text-[#8B5CF6] text-base font-bold ml-1">voltar</Text>
                </TouchableOpacity>

                <Text className="text-3xl font-bold text-black mb-8">
                    Cadastrar lote
                </Text>

                {/* Alojamento */}
                <View className="mb-8">
                    <Text className="text-[#9CA3AF] font-bold mb-2 uppercase text-sm">ALOJAMENTO</Text>

                    <Text className="text-black font-bold mb-1">Data</Text>
                    <TextInput
                        className="bg-gray-200 rounded-lg p-4 text-base text-gray-800"
                        placeholder="00/00/0000"
                        keyboardType="numeric"
                        value={accommodationDate}
                        onChangeText={handleDateChange}
                        maxLength={10}
                    />
                </View>

                {/* Linhagens Dinâmicas */}
                {lineages.map((item, index) => (
                    <View key={item.id} className="mb-6">
                        <Text className="text-[#9CA3AF] font-bold mb-2 uppercase text-sm">
                            LINHAGEM {String(index + 1).padStart(2, '0')}
                        </Text>

                        <Text className="text-black font-bold mb-1">Nome da raça</Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 mb-4"
                            value={item.raceName}
                            onChangeText={(text) => updateLineage(item.id, 'raceName', text)}
                        />

                        <Text className="text-black font-bold mb-1">Quantidade</Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800"
                            keyboardType="numeric"
                            value={item.quantity}
                            onChangeText={(text) => updateLineage(item.id, 'quantity', text)}
                        />
                    </View>
                ))}

                {/* Botão Adicionar Mais */}
                <TouchableOpacity
                    onPress={addLineage}
                    className="flex-row items-center mb-12"
                >
                    <Text className="text-[#9CA3AF] font-bold mr-2 uppercase text-sm">
                        CADASTRAR MAIS UMA LINHAGEM
                    </Text>
                    <Ionicons name="add" size={24} color="#9CA3AF" />
                </TouchableOpacity>

                {/* Submit Button */}
                <View className="mt-auto mb-4">
                    <TouchableOpacity
                        className="bg-[#8B5CF6] w-full py-4 rounded-full items-center shadow-lg shadow-purple-200"
                        onPress={handleRegister}
                        disabled={isLoading}
                    >
                        <Text className="text-white text-lg font-bold">
                            {isLoading ? 'Cadastrando...' : 'Cadastrar'}
                        </Text>
                    </TouchableOpacity>
                </View>

            </ScrollView>

            <SuccessModal visible={showSuccess} onClose={handleSuccessClose} />
        </SafeAreaView>
    );
}
