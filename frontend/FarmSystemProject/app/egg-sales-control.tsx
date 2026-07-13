import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, Alert, ActivityIndicator, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import AsyncStorage from '@react-native-async-storage/async-storage';

import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal';
import ReportResultModal from '@/components/ReportResultModal';

export default function EggSalesControlScreen() {
    const { id } = useLocalSearchParams(); 
    const router = useRouter();

    // Data inicial = Hoje
    const today = new Date();
    const formattedToday = `${String(today.getDate()).padStart(2, '0')}/${String(today.getMonth() + 1).padStart(2, '0')}/${today.getFullYear()}`;
    
    // --- ESTADOS DO FORMULÁRIO ---
    const [saleDate, setSaleDate] = useState(formattedToday);
    const [unitValue, setUnitValue] = useState('');
    const [eggQuantity, setEggQuantity] = useState('');

    // --- ESTADOS DE ERRO/VALIDAÇÃO ---
    const [verifyError, setVerifyError] = useState(false);
    
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
        if (setter === setVerifyDate) setVerifyError(false);
    };

    const parseCurrencyToFloat = (val: string) => {
        const clean = val.replace(/[^\d,-]/g, '').replace(',', '.');
        return parseFloat(clean);
    };

    // --- 1. REGISTRAR VENDA (POST) ---
    const handleRegister = async () => {
        if (!saleDate || !unitValue || !eggQuantity) {
            Alert.alert("Erro", "Preencha todos os campos do formulário.");
            return;
        }

        const isoDate = formatDateToISO(saleDate);
        if (!isoDate) { Alert.alert("Erro", "Data de venda inválida."); return; }

        const parsedUnitValue = parseCurrencyToFloat(unitValue);
        if (isNaN(parsedUnitValue) || parsedUnitValue <= 0) {
            Alert.alert("Erro", "Informe um valor unitário válido.");
            return;
        }

        setIsLoading(true);
        try {
            const token = await AsyncStorage.getItem('userToken');
            
            // Payload alinhado com o schema /api/lots/{lotId}/sales
            const payload = {
                unitValue: parsedUnitValue,
                eggQuantity: parseInt(eggQuantity) || 0,
                saleDate: new Date(isoDate).toISOString()
            };

            const response = await fetch(`${API_URL}/api/lots/${id}/sales`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                setShowSuccessModal(true);
                setUnitValue('');
                setEggQuantity('');
            } else {
                const errorData = await response.json().catch(() => ({}));
                Alert.alert("Erro", errorData.message || "Falha ao registrar venda de ovos.");
            }
        } catch (error) {
            console.error(error);
            Alert.alert("Erro", "Falha na conexão com o servidor.");
        } finally {
            setIsLoading(false);
        }
    };

    // --- 2. VERIFICAR DATA (GET summary) ---
    const handleVerify = async () => {
        if (!verifyDate) { Alert.alert("Atenção", "Informe uma data para verificar."); return; }
        const isoDate = formatDateToISO(verifyDate);
        if (!isoDate) { Alert.alert("Erro", "Data inválida."); return; }

        setIsLoading(true);
        setVerifyError(false);
        try {
            const token = await AsyncStorage.getItem('userToken');
            const response = await fetch(`${API_URL}/api/lots/${id}/sales/summary?date=${isoDate}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const data = await response.json();
                
                const totalCalculado = data.totalValue !== undefined 
                    ? data.totalValue 
                    : (data.eggQuantity || 0) * (data.unitValue || 0);
                
                // Labels sem o ':' final para evitar duplicação visual no modal
                setReportData([
                    { label: 'Valor Unidade', value: `R$ ${Number(data.unitValue || 0).toFixed(2).replace('.', ',')}` },
                    { label: 'Quantidade de Ovos', value: `${data.eggQuantity || 0}` },
                    { label: 'Valor Total', value: `R$ ${Number(totalCalculado).toFixed(2).replace('.', ',')}` }
                ]);
                
                setReportDateDisplay(verifyDate);
                setIsoReportDate(isoDate);
                setShowReportModal(true);
            } else {
                setVerifyError(true);
            }
        } catch (error) {
            console.error(error);
            Alert.alert("Erro", "Falha ao buscar dados do relatório.");
        } finally {
            setIsLoading(false);
        }
    };

    // --- 3. BAIXAR PDF (GET /pdf e GET /pdf/daily) ---
    const downloadPdf = async (type: 'general' | 'daily') => {
        try {
            setIsLoading(true);
            const token = await AsyncStorage.getItem('userToken');
            
            let url = `${API_URL}/api/lots/${id}/sales/pdf`;
            let filename = `Relatorio_Vendas_Ovos_Lote_${id}.pdf`;

            if (type === 'daily') {
                url = `${API_URL}/api/lots/${id}/sales/pdf/daily?date=${isoReportDate}`;
                filename = `Relatorio_Vendas_Ovos_${isoReportDate}.pdf`;
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
                    Alert.alert("Sucesso", "PDF gerado com sucesso.");
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

                <Text className="text-2xl font-bold text-black mb-6">Registro de Vendas</Text>

                {/* FORMULÁRIO DE ADIÇÃO */}
                <View className="space-y-4">
                    
                    {/* Campo: Valor Unitário */}
                    <View>
                        <Text className="text-black font-bold mb-1 text-base">Valor Unitário</Text>
                        <TextInput 
                            className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                            value={unitValue}
                            onChangeText={setUnitValue}
                            keyboardType="numeric"
                            placeholder="R$ 0,00"
                        />
                    </View>

                    {/* Campo: Quantidade de Ovos */}
                    <View>
                        <Text className="text-black font-bold mb-1 text-base">Quantidade de Ovos</Text>
                        <TextInput 
                            className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                            value={eggQuantity}
                            onChangeText={setEggQuantity}
                            keyboardType="numeric"
                            placeholder="0"
                        />
                    </View>

                    {/* Campo: Data de Venda */}
                    <View>
                        <Text className="text-black font-bold mb-1 text-base">Data de Venda</Text>
                        <View className="relative justify-center">
                            <TextInput 
                                className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm pr-10"
                                value={saleDate}
                                onChangeText={(t) => handleDateMask(t, setSaleDate)}
                                keyboardType="numeric"
                                placeholder="00/00/0000"
                                maxLength={10}
                            />
                            <MaterialCommunityIcons name="calendar-month" size={24} color="#8B5CF6" style={{ position: 'absolute', right: 10 }} />
                        </View>
                    </View>

                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-full items-center mt-6 shadow-md shadow-purple-200 w-40 self-center"
                        onPress={handleRegister}
                        disabled={isLoading}
                    >
                        {isLoading ? <ActivityIndicator color="white" /> : <Text className="text-white font-bold text-lg">Adicionar</Text>}
                    </TouchableOpacity>
                </View>

                {/* VERIFICAÇÃO / RELATÓRIO DIÁRIO */}
                <Text className="text-2xl font-bold text-black mt-12 mb-4">Relatório de Vendas</Text>
                
                <View className="mb-6">
                    <Text className="text-black font-bold mb-1 text-base">Data a verificar</Text>
                    <View className="relative justify-center">
                        <TextInput 
                            className={`bg-gray-200 rounded-lg p-3 text-black pr-10 text-base shadow-sm ${verifyError ? 'border border-red-500' : ''}`}
                            value={verifyDate}
                            onChangeText={(t) => handleDateMask(t, setVerifyDate)}
                            keyboardType="numeric"
                            placeholder="00/00/0000"
                            maxLength={10}
                        />
                        <MaterialCommunityIcons name="calendar-month" size={24} color="#8B5CF6" style={{ position: 'absolute', right: 10 }} />
                    </View>
                    
                    {verifyError && (
                        <Text className="text-red-600 font-bold text-xs mt-1">Informe uma data no formato aa/aa/aaaa</Text>
                    )}

                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-full items-center mt-6 shadow-md shadow-purple-200 w-40 self-center"
                        onPress={handleVerify}
                        disabled={isLoading}
                    >
                        <Text className="text-white font-bold text-lg">Verificar</Text>
                    </TouchableOpacity>
                </View>

                {/* RELATÓRIO GERAL */}
                <View className="mt-4 mb-8">
                    <Text className="text-2xl font-bold text-black mb-1">Relatório de Vendas</Text>
                    <Text className="text-gray-500 mb-4 text-sm font-bold leading-5">
                        Baixe o Relatório de Vendas Completo desse lote:
                    </Text>
                    
                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-2xl items-center w-40 shadow-md shadow-purple-200"
                        onPress={() => downloadPdf('general')}
                        disabled={isLoading}
                    >
                        <Text className="text-white font-bold text-lg">Baixar PDF</Text>
                    </TouchableOpacity>
                </View>

            </ScrollView>

            {/* MODAIS */}
            <SuccessModal 
                visible={showSuccessModal} 
                onClose={() => setShowSuccessModal(false)} 
                message="VENDA REGISTRADA COM SUCESSO"
            />

            <ReportResultModal 
                visible={showReportModal}
                onClose={() => setShowReportModal(false)}
                title="Relatório de Venda"
                dateDisplay={reportDateDisplay}
                data={reportData}
                onDownloadPdf={() => downloadPdf('daily')}
                isDownloading={isLoading}
            />

        </SafeAreaView>
    );
}