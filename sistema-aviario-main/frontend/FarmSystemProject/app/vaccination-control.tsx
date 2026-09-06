import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, Alert, ActivityIndicator, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import AsyncStorage from '@react-native-async-storage/async-storage';

import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal';
import ReportResultModal from '@/components/ReportResultModal';

export default function VaccinationControlScreen() {
    const { id } = useLocalSearchParams(); 
    const router = useRouter();

    // Data inicial = Hoje
    const today = new Date();
    const formattedToday = `${String(today.getDate()).padStart(2, '0')}/${String(today.getMonth() + 1).padStart(2, '0')}/${today.getFullYear()}`;
    
    // --- ESTADOS DO FORMULÁRIO ---
    const [date, setDate] = useState(formattedToday);
    const [vaccineType, setVaccineType] = useState('');
    const [unitValue, setUnitValue] = useState('');
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

    // Formata moeda (R$) apenas visualmente se quiser, mas aqui vou manter raw string para simplificar o parse
    // Se quiser máscara de moeda, avise.

    // --- 1. REGISTRAR VACINAÇÃO ---
    const handleRegister = async () => {
        if (!date || !vaccineType || !unitValue || !quantity) {
            Alert.alert("Erro", "Preencha todos os campos.");
            return;
        }
        const isoDate = formatDateToISO(date);
        if (!isoDate) { Alert.alert("Erro", "Data inválida."); return; }

        setIsLoading(true);
        try {
            const token = await AsyncStorage.getItem('userToken');
            
            // Tratamento de valores numéricos (troca vírgula por ponto se usuário digitar)
            const val = parseFloat(unitValue.replace(',', '.'));
            const qtd = parseInt(quantity);

            const payload = {
                applicationDate: new Date(isoDate).toISOString(),
                vaccineType: vaccineType,
                applicationValue: val || 0,
                applicationQuantity: qtd || 0
            };

            const response = await fetch(`${API_URL}/api/lots/${id}/vaccinations`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                setShowSuccessModal(true);
                // Limpa campos (mantém data)
                setVaccineType('');
                setUnitValue('');
                setQuantity('');
            } else {
                const errorData = await response.json();
                Alert.alert("Erro", errorData.message || "Falha ao registrar vacinação.");
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
            const response = await fetch(`${API_URL}/api/lots/${id}/vaccinations/summary?date=${isoDate}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const data = await response.json();
                
                // Mapeia para o Modal Genérico (Baseado na Imagem 02 do Figma de Vacinação)
                setReportData([
                    { label: 'Tipo', value: data.vaccineTypes || '-' },
                    // Formata valor para moeda BRL
                    { label: 'Valor uni', value: `R$${(data.applicationValue || 0).toFixed(2).replace('.', ',')}` },
                    { label: 'Quantidade', value: data.totalQuantity },
                    { label: 'Valor Total', value: `R$${(data.totalCost || 0).toFixed(2).replace('.', ',')}` }
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

    // --- 3. BAIXAR PDF (Web Compatible) ---
    const downloadPdf = async (type: 'general' | 'daily') => {
        try {
            setIsLoading(true);
            const token = await AsyncStorage.getItem('userToken');
            
            let url = `${API_URL}/api/lots/${id}/vaccinations/pdf`;
            let filename = `Vacinacao_Lote_${id}.pdf`;

            if (type === 'daily') {
                url = `${API_URL}/api/lots/${id}/vaccinations/pdf/daily?date=${isoReportDate}`;
                filename = `Vacinacao_${isoReportDate}.pdf`;
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

                <Text className="text-2xl font-bold text-black mb-6">Registro de Vacinação</Text>

                {/* FORMULÁRIO */}
                <View className="space-y-4">
                    <View>
                        <Text className="text-black font-bold mb-1 text-base">Data de aplicação</Text>
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
                        <Text className="text-black font-bold mb-1 text-base">Tipo de Vacina</Text>
                        <TextInput 
                            className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                            value={vaccineType}
                            onChangeText={setVaccineType}
                            placeholder=""
                        />
                    </View>

                    <View>
                        <Text className="text-black font-bold mb-1 text-base">Valor Individual da Aplicação</Text>
                        <TextInput 
                            className="bg-gray-200 rounded-lg p-3 text-black text-base shadow-sm"
                            value={unitValue}
                            onChangeText={setUnitValue}
                            keyboardType="numeric"
                            placeholder="0,00"
                        />
                    </View>

                    <View>
                        <Text className="text-black font-bold mb-1 text-base">Quantidade de Aplicações</Text>
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
                <Text className="text-2xl font-bold text-black mt-12 mb-4">Relatório de Vacinação</Text>
                
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
                    <Text className="text-2xl font-bold text-black mb-1">Relatório de Vacinação</Text>
                    <Text className="text-gray-500 mb-4 text-sm font-bold leading-5">
                        Baixe o Relatório de Vacinação Completo {'\n'}desse lote:
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
                title="Relatório de Vacinação"
                dateDisplay={reportDateDisplay}
                data={reportData}
                onDownloadPdf={() => downloadPdf('daily')}
                isDownloading={isLoading}
            />

        </SafeAreaView>
    );
}