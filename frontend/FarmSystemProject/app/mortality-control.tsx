import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, Alert, ActivityIndicator, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import AsyncStorage from '@react-native-async-storage/async-storage';

import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal';
import ReportResultModal from '@/components/ReportResultModal';

export default function MortalityControlScreen() {
    const { id } = useLocalSearchParams(); 
    const router = useRouter();

    // Data de hoje formatada para display
    const today = new Date();
    const formattedToday = `${String(today.getDate()).padStart(2, '0')}/${String(today.getMonth() + 1).padStart(2, '0')}/${today.getFullYear()}`;
    
    // Estados do Formulário
    const [date, setDate] = useState(formattedToday);
    const [deaths, setDeaths] = useState('');
    const [cuts, setCuts] = useState('');
    const [reason, setReason] = useState('');

    // Estados de Verificação e Modal
    const [verifyDate, setVerifyDate] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [showSuccessModal, setShowSuccessModal] = useState(false);
    
    const [showReportModal, setShowReportModal] = useState(false);
    const [reportData, setReportData] = useState<any[]>([]);
    const [reportDateDisplay, setReportDateDisplay] = useState('');
    const [isoReportDate, setIsoReportDate] = useState('');

    // --- Helpers ---
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

    // --- 1. REGISTRAR MORTALIDADE ---
    const handleRegister = async () => {
        if (!date || (!deaths && !cuts)) {
            Alert.alert("Erro", "Preencha a data e pelo menos uma quantidade.");
            return;
        }
        const isoDate = formatDateToISO(date);
        if (!isoDate) { Alert.alert("Erro", "Data inválida."); return; }

        setIsLoading(true);
        try {
            const token = await AsyncStorage.getItem('userToken');
            const payload = {
                dateDeath: new Date(isoDate).toISOString(),
                deathQuantity: parseInt(deaths) || 0,
                cutQuantity: parseInt(cuts) || 0,
                reason: reason || "Não informado"
            };

            const response = await fetch(`${API_URL}/api/lots/${id}/mortalities`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                setShowSuccessModal(true);
                setDeaths(''); setCuts(''); setReason('');
            } else {
                const errorData = await response.json();
                Alert.alert("Erro", errorData.message || "Falha ao registrar.");
            }
        } catch (error) {
            console.error(error);
            Alert.alert("Erro", "Falha na conexão.");
        } finally {
            setIsLoading(false);
        }
    };

    // --- 2. VERIFICAR DATA (ABRE MODAL) ---
    const handleVerify = async () => {
        if (!verifyDate) { Alert.alert("Atenção", "Informe uma data."); return; }
        const isoDate = formatDateToISO(verifyDate);
        if (!isoDate) { Alert.alert("Erro", "Data inválida."); return; }

        setIsLoading(true);
        try {
            const token = await AsyncStorage.getItem('userToken');
            const response = await fetch(`${API_URL}/api/lots/${id}/mortalities/summary?date=${isoDate}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const data = await response.json();
                
                // Monta dados para o Modal
                setReportData([
                    { label: 'Mortes', value: data.totalDeaths },
                    { label: 'Corte', value: data.totalCuts },
                    { label: 'Motivo', value: data.motives || '-' },
                    { label: 'Taxa de Mortalidade', value: `${data.mortalityRate}%` }
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

    // --- 3. BAIXAR PDF (LÓGICA WEB PURA) ---
    const downloadPdf = async (type: 'general' | 'daily') => {
        try {
            setIsLoading(true);
            const token = await AsyncStorage.getItem('userToken');
            
            let url = `${API_URL}/api/lots/${id}/mortalities/pdf`;
            let filename = `Relatorio_Mortalidade_Lote_${id}.pdf`;

            if (type === 'daily') {
                url = `${API_URL}/api/lots/${id}/mortalities/pdf/daily?date=${isoReportDate}`;
                filename = `Mortalidade_${isoReportDate}.pdf`;
            }

            // Faz o fetch esperando um BLOB (arquivo binário)
            const response = await fetch(url, {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                // LÓGICA DE DOWNLOAD PARA WEB
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
                    Alert.alert("Aviso", "Download nativo não implementado nesta versão web-first.");
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

                <Text className="text-2xl font-bold text-black mb-6">Controle de Mortalidade</Text>

                {/* FORMULÁRIO */}
                <View className="space-y-4">
                    <View>
                        <Text className="text-black font-bold mb-1 ml-1 text-base">Data</Text>
                        <TextInput 
                            className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                            value={date}
                            onChangeText={(t) => handleDateMask(t, setDate)}
                            keyboardType="numeric"
                            placeholder="DD/MM/AAAA"
                            maxLength={10}
                        />
                    </View>

                    <View className="flex-row justify-between">
                        <View className="w-[48%]">
                            <Text className="text-black font-bold mb-1 ml-1 text-base">Morte</Text>
                            <TextInput 
                                className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                                value={deaths}
                                onChangeText={setDeaths}
                                keyboardType="numeric"
                                placeholder="0"
                            />
                        </View>
                        <View className="w-[48%]">
                            <Text className="text-black font-bold mb-1 ml-1 text-base">Corte</Text>
                            <TextInput 
                                className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                                value={cuts}
                                onChangeText={setCuts}
                                keyboardType="numeric"
                                placeholder="0"
                            />
                        </View>
                    </View>

                    <View>
                        <Text className="text-black font-bold mb-1 ml-1 text-base">Motivo</Text>
                        <TextInput 
                            className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                            value={reason}
                            onChangeText={setReason}
                            placeholder=""
                        />
                    </View>

                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-full items-center mt-4 shadow-md shadow-purple-200"
                        onPress={handleRegister}
                        disabled={isLoading}
                    >
                        {isLoading ? <ActivityIndicator color="white" /> : <Text className="text-white font-bold text-lg">Atualizar</Text>}
                    </TouchableOpacity>
                </View>

                {/* VERIFICAÇÃO */}
                <Text className="text-2xl font-bold text-black mt-12 mb-4">Relatório de Mortalidade</Text>
                
                <View className="mb-6">
                    <Text className="text-black font-bold mb-1 ml-1 text-base">Data a verificar</Text>
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
                        className="bg-[#8B5CF6] py-4 rounded-full items-center mt-6 shadow-md shadow-purple-200"
                        onPress={handleVerify}
                    >
                        <Text className="text-white font-bold text-lg">Verificar</Text>
                    </TouchableOpacity>
                </View>

                {/* RELATÓRIO GERAL */}
                <View className="mt-4 mb-8">
                    <Text className="text-2xl font-bold text-black mb-1">Relatório de Mortalidade</Text>
                    <Text className="text-gray-500 mb-4 text-sm font-bold leading-5">
                        Baixe o Relatório de Mortalidade Completo {'\n'}desse lote:
                    </Text>
                    
                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-2xl items-center w-40 shadow-md shadow-purple-200"
                        onPress={() => downloadPdf('general')}
                    >
                        <Text className="text-white font-bold text-lg">Baixar PDF</Text>
                    </TouchableOpacity>
                </View>

            </ScrollView>

            <SuccessModal 
                visible={showSuccessModal} 
                onClose={() => setShowSuccessModal(false)} 
                message="ATUALIZADO COM SUCESSO"
            />

            <ReportResultModal 
                visible={showReportModal}
                onClose={() => setShowReportModal(false)}
                title="Relatório de Mortalidade" // Nome que aparece no Modal (Imagem 02)
                dateDisplay={reportDateDisplay}
                data={reportData}
                onDownloadPdf={() => downloadPdf('daily')}
                isDownloading={isLoading}
            />

        </SafeAreaView>
    );
}