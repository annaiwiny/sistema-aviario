import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, ActivityIndicator, Platform, Modal } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import AsyncStorage from '@react-native-async-storage/async-storage';

import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal';
import ReportResultModal from '@/components/ReportResultModal';
import { showAlert } from '@/utils/alert';

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
    const [successMessage, setSuccessMessage] = useState('COLETA REGISTRADA COM SUCESSO');

    // Coleta que já existe na data digitada. Enquanto estiver preenchida, o
    // usuário confirma que quer SUBSTITUIR o total do dia. Somar não existe
    // mais: era o que fazia o total estourar a cada lançamento repetido.
    const [conflict, setConflict] = useState<{ isoDate: string; dateDisplay: string; existing: number; typed: number } | null>(null);
    
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

    // Total já lançado na data, ou null se ainda não houver nada.
    const fetchExistingTotal = async (isoDate: string): Promise<number | null> => {
        const token = await AsyncStorage.getItem('userToken');
        const response = await fetch(`${API_URL}/api/lots/${id}/eggs/summary?date=${isoDate}&_=${Date.now()}`, {
            cache: 'no-store',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        // 404 = dia sem lançamento nenhum, que aqui não é erro.
        if (!response.ok) return null;

        const data = await response.json();
        return data.totalQuantity ?? null;
    };

    // Mensagem de erro da API. Nem toda resposta de erro traz JSON: um 405 do
    // ASP.NET vem sem corpo e uma página de erro do proxy vem em HTML. Sem
    // isolar o parse, o `json()` estourava e o erro real virava "falha na
    // conexão", que manda procurar o problema no lugar errado.
    const readErrorMessage = async (response: Response, fallback: string) => {
        try {
            const data = await response.json();
            if (data?.message) return data.message;
        } catch {
            // corpo vazio ou não-JSON: sobra o status, que já diz muita coisa
        }
        return `${fallback} (erro ${response.status})`;
    };

    // Grava a coleta. O valor digitado é sempre o TOTAL do dia, nunca uma
    // parcela a somar: PUT corrige um dia que já tem lançamento, POST cria o
    // primeiro. No servidor os dois terminam no mesmo lugar - um dia, um
    // lançamento -, então o relatório mostra só o valor atualizado.
    const saveEntry = async (isoDate: string, value: number, mode: 'create' | 'replace') => {
        setIsLoading(true);
        try {
            const token = await AsyncStorage.getItem('userToken');

            const payload = {
                productionDate: new Date(isoDate).toISOString(),
                quantity: value
            };

            const response = await fetch(`${API_URL}/api/lots/${id}/eggs`, {
                method: mode === 'replace' ? 'PUT' : 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                setSuccessMessage(mode === 'replace'
                    ? 'COLETA ATUALIZADA COM SUCESSO'
                    : 'COLETA REGISTRADA COM SUCESSO');
                setShowSuccessModal(true);
                setQuantity(''); // Limpa quantidade, mantém data
            } else {
                // O backend pode retornar erro se Qtd > Galinhas Vivas
                showAlert("Erro", await readErrorMessage(response, "Falha ao registrar produção."));
            }
        } catch (error) {
            console.error(error);
            showAlert("Erro", "Falha na conexão.");
        } finally {
            setIsLoading(false);
        }
    };

    // --- 1. REGISTRAR PRODUÇÃO ---
    const handleRegister = async () => {
        if (!date || !quantity) {
            showAlert("Erro", "Preencha a data e a quantidade.");
            return;
        }
        const isoDate = formatDateToISO(date);
        if (!isoDate) { showAlert("Erro", "Data inválida."); return; }

        const typed = parseInt(quantity) || 0;

        setIsLoading(true);
        try {
            const existing = await fetchExistingTotal(isoDate);

            if (existing !== null) {
                // Já tem coleta nesse dia: confirma antes de trocar o valor.
                setConflict({ isoDate, dateDisplay: date, existing, typed });
                return;
            }
        } catch (error) {
            console.error(error);
            showAlert("Erro", "Falha na conexão.");
            return;
        } finally {
            setIsLoading(false);
        }

        await saveEntry(isoDate, typed, 'create');
    };

    // --- 2. VERIFICAR DATA ---
    const handleVerify = async () => {
        if (!verifyDate) { showAlert("Atenção", "Informe uma data."); return; }
        const isoDate = formatDateToISO(verifyDate);
        if (!isoDate) { showAlert("Erro", "Data inválida."); return; }

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
                showAlert("Aviso", "Nenhum registro para esta data.");
            }
        } catch (error) {
            console.error(error);
            showAlert("Erro", "Falha ao buscar dados.");
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
                    showAlert("Sucesso", "PDF gerado (Mobile pendente).");
                }
            } else {
                showAlert("Erro", "Falha ao gerar o PDF.");
            }
        } catch (error) {
            console.error(error);
            showAlert("Erro", "Não foi possível baixar o PDF.");
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
                        <Text className="text-gray-500 text-xs mt-1 leading-4">
                            Digite o total do dia. Se o dia já tiver lançamento, este valor
                            substitui o anterior.
                        </Text>
                    </View>

                    <TouchableOpacity 
                        className="bg-[#8B5CF6] py-4 rounded-full items-center mt-6 shadow-md shadow-purple-200 w-40 self-center"
                        onPress={handleRegister}
                        disabled={isLoading}
                    >
                        {isLoading ? <ActivityIndicator color="white" /> : <Text className="text-white font-bold text-lg">Salvar</Text>}
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
            {/* Confirmação de que o valor do dia vai ser substituído */}
            <Modal
                animationType="fade"
                transparent
                visible={conflict !== null}
                onRequestClose={() => setConflict(null)}
            >
                <View className="flex-1 bg-black/50 justify-center items-center px-6">
                    <View className="bg-white rounded-[32px] p-7 w-full max-w-[340px] elevation-5 shadow-lg">
                        <Text className="text-xl font-black text-black mb-2 text-center">
                            Atualizar a coleta do dia?
                        </Text>
                        <Text className="text-gray-600 text-center text-base mb-6 leading-5">
                            Em {conflict?.dateDisplay} já constam{' '}
                            <Text className="font-bold text-black">{conflict?.existing} ovos</Text>.
                            {'\n'}O dia passa a valer{' '}
                            <Text className="font-bold text-black">{conflict?.typed} ovos</Text>.
                            {' '}O valor antigo é substituído, não somado.
                        </Text>

                        <TouchableOpacity
                            className="bg-[#8B5CF6] w-full py-4 rounded-full items-center mb-3 shadow-md shadow-purple-200"
                            disabled={isLoading}
                            onPress={() => {
                                const c = conflict;
                                setConflict(null);
                                if (c) saveEntry(c.isoDate, c.typed, 'replace');
                            }}
                        >
                            <Text className="text-white font-bold text-base">
                                Atualizar para {conflict?.typed}
                            </Text>
                        </TouchableOpacity>

                        <TouchableOpacity className="w-full py-3 items-center" onPress={() => setConflict(null)}>
                            <Text className="text-gray-500 font-bold text-base">Cancelar</Text>
                        </TouchableOpacity>
                    </View>
                </View>
            </Modal>

            <SuccessModal 
                visible={showSuccessModal} 
                onClose={() => setShowSuccessModal(false)} 
                message={successMessage}
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