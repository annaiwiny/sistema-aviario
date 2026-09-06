import React, { useEffect, useState } from 'react';
import { View, Text, TouchableOpacity, ScrollView, ActivityIndicator, Alert, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_URL } from '@/constants/Api';

interface SensorData {
    type: string;
    value: string;
    measuredAt: string;
    status: string;
}

export default function SensorsControlScreen() {
    const { id } = useLocalSearchParams();
    const router = useRouter();

    const [loading, setLoading] = useState(true);
    const [downloadingType, setDownloadingType] = useState<number | null>(null);
    const [sensors, setSensors] = useState<SensorData[]>([]);

    // --- 1. BUSCAR DADOS DOS SENSORES (GET summary) ---
    const fetchSensorsSummary = async () => {
        try {
            const token = await AsyncStorage.getItem('userToken');
            const response = await fetch(`${API_URL}/api/lots/${id}/sensors/summary`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const data = await response.json();
                setSensors(data);
            } else {
                Alert.alert('Erro', 'Não foi possível carregar os dados dos sensores.');
            }
        } catch (error) {
            console.error(error);
            Alert.alert('Erro', 'Falha na conexão com o servidor.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (id) {
            fetchSensorsSummary();
        }
    }, [id]);

    // --- 2. GERAR RELATÓRIO PDF POR TIPO (GET report/{type}) ---
    const downloadReport = async (sensorTypeEnum: number, sensorName: string) => {
        try {
            setDownloadingType(sensorTypeEnum);
            const token = await AsyncStorage.getItem('userToken');
            
            // Rota da API: /api/lots/{lotId}/sensors/report/{type}
            const url = `${API_URL}/api/lots/${id}/sensors/report/${sensorTypeEnum}`;
            const filename = `Relatorio_${sensorName}_Lote_${id}.pdf`;

            const response = await fetch(url, { 
                headers: { 'Authorization': `Bearer ${token}` } 
            });

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
                    Alert.alert("Sucesso", `Relatório de ${sensorName} gerado com sucesso.`);
                }
            } else {
                Alert.alert("Erro", "Falha ao gerar o relatório do sensor.");
            }
        } catch (error) {
            console.error(error);
            Alert.alert("Erro", "Não foi possível baixar o relatório.");
        } finally {
            setDownloadingType(null);
        }
    };

    // --- 3. CONFIGURAÇÃO VISUAL E MAPEAMENTO POR TIPO DE SENSOR ---
    const getSensorConfig = (typeName: string) => {
        const lower = typeName.toLowerCase();
        if (lower.includes('umidade')) {
            return {
                title: 'Umidade',
                typeEnum: 2,
                idealText: 'Níveis ideais de umidade ficam entre 50% e 70%'
            };
        }
        if (lower.includes('temp')) {
            return {
                title: 'Temperatura',
                typeEnum: 1,
                idealText: 'Níveis ideais de temperatura ficam entre 18º e 24º'
            };
        }
        // Padrão para Nível de Água
        return {
            title: 'Nível de água',
            typeEnum: 3,
            idealText: 'Nível ideal de água fica acima de 41%'
        };
    };

    // --- 4. CONFIGURAÇÃO DE CORES E ÍCONES POR STATUS ---
    const getStatusTheme = (status: string) => {
        const lower = status?.toLowerCase() || '';
        if (lower === 'ideal') {
            return { color: '#16A34A', icon: 'emoticon-happy', isNoData: false }; // Verde
        }
        if (lower === 'atenção' || lower === 'atencao') {
            return { color: '#D97706', icon: 'emoticon-neutral', isNoData: false }; // Amarelo/Laranja
        }
        if (lower === 'crítico' || lower === 'critico') {
            return { color: '#DC2626', icon: 'emoticon-sad', isNoData: false }; // Vermelho
        }
        // Retorno para "Sem Dados" ou qualquer outro não previsto
        return { color: '#6B7280', icon: 'help-circle', isNoData: true }; // Cinza
    };

    if (loading) {
        return (
            <SafeAreaView className="flex-1 justify-center items-center bg-white">
                <ActivityIndicator size="large" color="#8B5CF6" />
            </SafeAreaView>
        );
    }

    return (
        <SafeAreaView className="flex-1 bg-white">
            <View className="px-6 py-4">
                <TouchableOpacity onPress={() => router.back()} className="flex-row items-center -ml-2">
                    <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                    <Text className="text-[#8B5CF6] font-bold text-base ml-1">voltar</Text>
                </TouchableOpacity>
            </View>

            <ScrollView contentContainerStyle={{ paddingHorizontal: 24, paddingBottom: 32 }}>
                
                {/* LISTAGEM DOS SENSORES */}
                {sensors.map((item, index) => {
                    const config = getSensorConfig(item.type);
                    const theme = getStatusTheme(item.status);
                    const isDownloadingThis = downloadingType === config.typeEnum;

                    return (
                        <View key={index} className="mb-8 border-b border-gray-100 pb-6">
                            
                            {/* Título do Sensor */}
                            <Text className="text-2xl font-bold text-black mb-2">
                                {config.title}
                            </Text>

                            {/* Linha do Valor + Ícone (Carinha) */}
                            <View className="flex-row items-center justify-between mb-2">
                                {theme.isNoData ? (
                                    <Text className="text-2xl font-bold text-gray-500">
                                        Dados não coletados
                                    </Text>
                                ) : (
                                    <Text 
                                        className="text-6xl font-extrabold tracking-tight"
                                        style={{ color: theme.color }}
                                    >
                                        {item.value}
                                    </Text>
                                )}

                                {/* Ícone simulando as carinhas do protótipo */}
                                <MaterialCommunityIcons 
                                    name={theme.icon as any} 
                                    size={72} 
                                    color={theme.color} 
                                />
                            </View>

                            {/* Subtítulo Cinza com Níveis Ideais */}
                            <Text className="text-gray-400 font-medium text-xs mb-4">
                                {config.idealText}
                            </Text>

                            {/* Botão Gerar Relatório */}
                            <TouchableOpacity 
                                className="bg-[#8B5CF6] py-4 rounded-full items-center shadow-md shadow-purple-200 w-48"
                                onPress={() => downloadReport(config.typeEnum, config.title)}
                                disabled={downloadingType !== null}
                            >
                                {isDownloadingThis ? (
                                    <ActivityIndicator color="white" />
                                ) : (
                                    <Text className="text-white font-bold text-base">
                                        Gerar relatório
                                    </Text>
                                )}
                            </TouchableOpacity>

                        </View>
                    );
                })}

                {/* Mensagem caso o backend retorne array vazio */}
                {sensors.length === 0 && (
                    <View className="items-center justify-center mt-12">
                        <Text className="text-gray-500 font-bold">Nenhum sensor encontrado para este lote.</Text>
                    </View>
                )}

            </ScrollView>
        </SafeAreaView>
    );
}