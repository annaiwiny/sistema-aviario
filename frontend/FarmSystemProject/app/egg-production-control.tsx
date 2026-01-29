import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, Alert, ActivityIndicator, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import AsyncStorage from '@react-native-async-storage/async-storage';

import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal';
import ReportResultModal from '@/components/ReportResultModal';

export default function EggProductionControlScreen() {
    const { id } = useLocalSearchParams(); 
    const router = useRouter();

    // Data inicial = Hoje
    const today = new Date();
    const formattedToday = `${String(today.getDate()).padStart(2, '0')}/${String(today.getMonth() + 1).padStart(2, '0')}/${today.getFullYear()}`;
    
    // --- ESTADOS DO FORMULÁRIO ---
    const [date, setDate] = useState(formattedToday);
    const [quantity, setQuantity] = useState('');

    // --- ESTADOS DE VERIFICAÇÃO ---
    const [verifyDate, setVerifyDate] = useState('');
    
    // --- CONTROLES ---
    const [isLoading, setIsLoading] = useState(false);
    const [showSuccessModal, setShowSuccessModal] = useState(false);
    
    // --- MODAL DE RELATÓRIO ---
    const [showReportModal, setShowReportModal] = useState(false);
    const [reportData, setReportData] = useState<any[]>([]);
    const [reportDateDisplay, setReportDateDisplay] = useState('');
    const [isoReportDate, setIsoReportDate] = useState('');

    // --- HELPERS ---
    const formatDateToISO = (displayDate: string) => {
        const parts = displayDate.split('/');
        if (parts.length === 3) return `${parts[2]}-${parts[1]}-${parts[0]}`;
        return null;
    };

    const handleDateMask = (text: string, setter: (v: string) => void) => {
        let v = text.replace(/\D/g, '');
        if (v.length > 2) v = v.replace(/^(\d{2})(\d)/, '$1/$2');
        if (v.length > 5) v = v.replace(/^(\d{2})\/(\d{2})(\d)/, '$1/$2/$3');
        if (v.length > 10) v = v.slice(0, 10);
        setter(v);
    };

    // --- 1. REGISTRAR PRODUÇÃO ---
    const handleRegister = async () => {
        if (!date || !quantity) {
            Alert.alert("Erro", "Preencha a data e a quantidade.");
            return;
        }
        const isoDate = formatDateToISO(date);
        if (!isoDate) { Alert.alert("Erro", "Data inválida."); return; }

        setIsLoading(true);
        try {
            const token = await AsyncStorage.getItem('userToken');
            
            const payload = {
                productionDate: new Date(isoDate).toISOString(),
                quantity: parseInt(quantity) || 0
            };

            const response = await fetch(`${API_URL}/api/lots/${id}/eggs`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                setShowSuccessModal(true);
                setQuantity(''); // Limpa quantidade, mantém data
            } else {
                const errorData = await response.json();
                // O backend pode retornar erro se Qtd > Galinhas Vivas
                Alert.alert("Erro", errorData.message || "Falha ao registrar produção.");
            }
        } catch (error) {
            console.error(error);
            Alert.alert("Erro", "Falha na conexão.");
        } finally {
            setIsLoading(false);
        }
    };

    // --- 2. VERIFICAR DATA ---
    const handleVerify = async () => {
        if (!verifyDate) { Alert.alert("Atenção", "Informe uma data."); return; }
        const isoDate = formatDateToISO(verifyDate);
        if (!isoDate) { Alert.alert("Erro", "Data inválida."); return; }

        setIsLoading(true);
        try {
            const token = await AsyncStorage.getItem('userToken');
            const response = await fetch(`${API_URL}/api/lots/${id}/eggs/summary?date=${isoDate}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const data = await response.json();
                
                // Mapeia para o Modal Genérico (Baseado na Imagem 02 do Figma de Produção)
                setReportData([
                    { label: 'Ovos coletados', value: data.totalQuantity }
                ]);
                
                setReportDateDisplay(verifyDate);
                setIsoReportDate(isoDate);
                setShowReportModal(true);
            } else {
                Alert.alert("Aviso", "Nenhum registro para esta data.");
            }
        } catch (error) {
            console.error(error);
            Alert.alert("Erro", "Falha ao buscar dados.");
        } finally {
            setIsLoading(false);
        }
    };

    // --- 3. BAIXAR PDF ---
    const downloadPdf = async (type: 'general' | 'daily') => {
        try {
            setIsLoading(true);
            const token = await AsyncStorage.getItem('userToken');
            
            let url = `${API_URL}/api/lots/${id}/eggs/pdf`;
            let filename = `ProducaoOvos_Lote_${id}.pdf`;

            if (type === 'daily') {
                url = `${API_URL}/api/lots/${id}/eggs/pdf/daily?date=${isoReportDate}`;
                filename = `ProducaoOvos_${isoReportDate}.pdf`;
            }

            const response = await fetch(url, { headers: { 'Authorization': `Bearer ${token}` } });

            if (response.ok) {
                if (Platform.OS === 'web') {
                    const blob = await response.blob();
                    const downloadUrl = window.URL.createObjectURL(blob);
                    const link = document.createElement('a');
                    link.href = downloadUrl;
                    link.download = filename;
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    window.URL.revokeObjectURL(downloadUrl);
                } else {
                    Alert.alert("Sucesso", "PDF gerado (Mobile pendente).");
                }
            } else {
                Alert.alert("Erro", "Falha ao gerar o PDF.");
            }
        } catch (error) {
            console.error(error);
            Alert.alert("Erro", "Não foi possível baixar o PDF.");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <ScrollView contentContainerStyle={{ padding: 24 }}>
                
                {/* Header */}
                <TouchableOpacity onPress={() => router.back()} className="flex-row items-center mb-6">
                    <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                    <Text className="text-[#8B5CF6] font-bold text-base ml-1">voltar</Text>
                </TouchableOpacity>

                <Text className="text-2xl font-bold text-black mb-6">Produção de Ovos</Text>

                {/* FORMULÁRIO */}
                <View className="space-y-4">
                    <View>
                        <Text className="text-black font-bold mb-1 text-base">Data</Text>
                        <View className="relative justify-center">
                            <TextInput 
                                className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm pr-10"
                                value={date}
                                onChangeText={(t) => handleDateMask(t, setDate)}
                                keyboardType="numeric"
                                placeholder="DD/MM/AAAA"
                                maxLength={10}
                            />
                            <MaterialCommunityIcons name="calendar-month" size={24} color="#8B5CF6" style={{ position: 'absolute', right: 10 }} />
                        </View>
                    </View>

                    <View>
                        <Text className="text-black font-bold mb-1 text-base">Quantidade de Ovos Coletados</Text>
                        <TextInput 
                            className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                            value={quantity}
                            onChangeText={setQuantity}
                            keyboardType="numeric"
                            placeholder="0"
                        />
                    </View>

                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-full items-center mt-6 shadow-md shadow-purple-200 w-40 self-center"
                        onPress={handleRegister}
                        disabled={isLoading}
                    >
                        {isLoading ? <ActivityIndicator color="white" /> : <Text className="text-white font-bold text-lg">Adicionar</Text>}
                    </TouchableOpacity>
                </View>

                {/* VERIFICAÇÃO */}
                <Text className="text-2xl font-bold text-black mt-12 mb-4">Relatório de Coleta</Text>
                
                <View className="mb-6">
                    <Text className="text-black font-bold mb-1 text-base">Data a verificar</Text>
                    <View className="relative justify-center">
                        <TextInput 
                            className="bg-gray-200 rounded-lg p-3 text-black pr-10 text-base shadow-sm"
                            value={verifyDate}
                            onChangeText={(t) => handleDateMask(t, setVerifyDate)}
                            keyboardType="numeric"
                            placeholder="00/00/0000"
                            maxLength={10}
                        />
                        <MaterialCommunityIcons name="calendar-month" size={24} color="#8B5CF6" style={{ position: 'absolute', right: 10 }} />
                    </View>

                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-full items-center mt-6 shadow-md shadow-purple-200 w-40 self-center"
                        onPress={handleVerify}
                    >
                        <Text className="text-white font-bold text-lg">Verificar</Text>
                    </TouchableOpacity>
                </View>

                {/* RELATÓRIO GERAL */}
                <View className="mt-4 mb-8">
                    <Text className="text-2xl font-bold text-black mb-1">Relatório Geral</Text>
                    <Text className="text-gray-500 mb-4 text-sm font-bold leading-5">
                        Baixe o Relatório de Coleta Completo desse {'\n'}lote:
                    </Text>
                    
                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-2xl items-center w-40 shadow-md shadow-purple-200"
                        onPress={() => downloadPdf('general')}
                    >
                        <Text className="text-white font-bold text-lg">Baixar PDF</Text>
                    </TouchableOpacity>
                </View>

            </ScrollView>

            {/* MODAIS */}
            <SuccessModal 
                visible={showSuccessModal} 
                onClose={() => setShowSuccessModal(false)} 
                message="ADIÇÃO REALIZADA COM SUCESSO"
            />

            <ReportResultModal 
                visible={showReportModal}
                onClose={() => setShowReportModal(false)}
                title="Relatório de Produção de Ovos"
                dateDisplay={reportDateDisplay}
                data={reportData}
                onDownloadPdf={() => downloadPdf('daily')}
                isDownloading={isLoading}
            />

        </SafeAreaView>
    );
}