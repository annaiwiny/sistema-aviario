import React, { useEffect, useState } from 'react';
import { View, Text, TouchableOpacity, ScrollView, ActivityIndicator, Alert } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import Svg, { Path, Circle, Text as SvgText } from 'react-native-svg';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_URL } from '@/constants/Api';

interface LotDashboardData {
    lotId: number;
    currentAlive: number;
    eggsCollectedToday: number;
    hensNotLayingToday: number;
    layingPercentage: number;
}

export default function LotDetailsScreen() {
    const { id } = useLocalSearchParams();
    const router = useRouter();
    
    const [loading, setLoading] = useState(true);
    const [data, setData] = useState<LotDashboardData | null>(null);

    useEffect(() => {
        const fetchDashboard = async () => {
            try {
                const token = await AsyncStorage.getItem('userToken');
                const response = await fetch(`${API_URL}/api/Lot/${id}/dashboard`, {
                    headers: { 'Authorization': `Bearer ${token}` }
                });

                if (response.ok) {
                    const result = await response.json();
                    setData(result);
                } else {
                    Alert.alert('Erro', 'Não foi possível carregar os dados do lote.');
                }
            } catch (error) {
                console.error(error);
                Alert.alert('Erro', 'Falha na conexão com o servidor.');
            } finally {
                setLoading(false);
            }
        };

        if (id) fetchDashboard();
    }, [id]);

    const PieChart = () => {
        if (!data) return null;

        const radius = 80; // Raio do gráfico
        const centerX = 100;
        const centerY = 100;
        const percentage = data.layingPercentage;

        // --- CÁLCULO DAS POSIÇÕES DOS TEXTOS ---
        
        // Distância do centro onde o texto vai ficar (meio do raio)
        const textRadius = radius * 0.6; 

        // 1. Posição do Texto Verde (Ovos)
        // O ângulo é metade da fatia verde. 
        // -90 graus para ajustar o início para o topo (12h)
        const greenAngle = (percentage / 100) * 360;
        const greenTextAngle = greenAngle / 2; 
        const greenRad = (greenTextAngle - 90) * (Math.PI / 180);
        
        const greenTextX = centerX + textRadius * Math.cos(greenRad);
        const greenTextY = centerY + textRadius * Math.sin(greenRad);

        // 2. Posição do Texto Branco (Não Botaram)
        // O ângulo é o fim da verde + metade da fatia roxa
        const purpleTextAngle = greenAngle + ((360 - greenAngle) / 2);
        const purpleRad = (purpleTextAngle - 90) * (Math.PI / 180);

        const purpleTextX = centerX + textRadius * Math.cos(purpleRad);
        const purpleTextY = centerY + textRadius * Math.sin(purpleRad);

        // --- LÓGICA DO DESENHO DO ARCO VERDE ---
        const angleRad = (greenAngle - 90) * (Math.PI / 180);
        const endX = centerX + radius * Math.cos(angleRad);
        const endY = centerY + radius * Math.sin(angleRad);
        const largeArcFlag = greenAngle > 180 ? 1 : 0;

        // Path da fatia verde
        const greenPath = `
            M ${centerX} ${centerY}
            L ${centerX} ${centerY - radius}
            A ${radius} ${radius} 0 ${largeArcFlag} 1 ${endX} ${endY}
            Z
        `;

        return (
            <View className="items-center justify-center my-4">
                <Svg height="200" width="200" viewBox="0 0 200 200">
                    {/* Fundo Roxo (Total) */}
                    <Circle cx={centerX} cy={centerY} r={radius} fill="#8B5CF6" />
                    
                    {/* Fatia Verde (Produzindo) */}
                    {percentage > 0 && percentage < 100 && (
                        <Path d={greenPath} fill="#D1FAE5" />
                    )}
                    {/* Se for 100%, desenha círculo verde cheio */}
                    {percentage >= 100 && (
                        <Circle cx={centerX} cy={centerY} r={radius} fill="#D1FAE5" />
                    )}

                    {/* TEXTOS DINÂMICOS */}
                    {/* Só mostra o texto se a fatia for grande o suficiente para não encavalar */}
                    
                    {/* Texto Verde (Produção) */}
                    {percentage > 5 && (
                        <SvgText 
                            fill="#15803d" // Verde escuro para contraste
                            fontSize="16" 
                            fontWeight="bold" 
                            x={greenTextX} 
                            y={greenTextY + 5} // +5 para ajuste vertical fino
                            textAnchor="middle"
                        >
                            {data.eggsCollectedToday}
                        </SvgText>
                    )}

                    {/* Texto Branco (Não Produção) */}
                    {percentage < 95 && (
                        <SvgText 
                            fill="white" 
                            fontSize="16" 
                            fontWeight="bold" 
                            x={purpleTextX} 
                            y={purpleTextY + 5} 
                            textAnchor="middle"
                        >
                            {data.hensNotLayingToday}
                        </SvgText>
                    )}
                </Svg>
                <Text className="font-bold text-black mt-2">POSTURA: {data.layingPercentage}%</Text>
            </View>
        );
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
            <View className="flex-row items-center justify-between px-6 py-4">
                <TouchableOpacity
                    className="flex-row items-center z-10 p-2 -ml-2"
                    onPress={() => router.canGoBack() ? router.back() : router.replace('/dashboard')}
                >
                    <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                    <Text className="text-[#8B5CF6] font-bold text-lg ml-1">voltar</Text>
                </TouchableOpacity>

                <View className="flex-row items-center">
                    <Text className="text-2xl font-bold text-black uppercase mr-2">
                        LOTE {String(data?.lotId || id).padStart(2, '0')}
                    </Text>
                    <TouchableOpacity onPress={() => router.push(`/edit-lot?id=${id}`)}>
                        <Ionicons name="pencil" size={20} color="#8B5CF6" />
                    </TouchableOpacity>
                </View>
                <View style={{ width: 40 }} />
            </View>

            <ScrollView contentContainerStyle={{ flexGrow: 1, paddingHorizontal: 24, paddingBottom: 24 }}>
                <PieChart />

                <View className="bg-[#D1FAE5] rounded-full py-3 px-6 mb-8 items-center justify-center">
                    <Text className="text-green-800 font-bold uppercase text-xs text-center">
                        {data?.eggsCollectedToday || 0} GALINHAS ESTÃO COLOCANDO OVO
                    </Text>
                </View>

                <View className="bg-[#D1FAE5] rounded-3xl p-6 py-10 space-y-4 gap-4">
                    <MenuButton title="Coleta Diária de Ovos" onPress={() => router.push(`/egg-production-control?id=${id}`)}/>
                    <MenuButton title="Vacinação" onPress={() => router.push(`/vaccination-control?id=${id}`)}/>
                    <MenuButton title="Controle de Alimentação" onPress={() => router.push(`/feeding-control?id=${id}`)}/>
                    <MenuButton title="Controle de Gastos" onPress={() => router.push(`/feed-cost-control?id=${id}`)}/>
                    <MenuButton title="Controle de Mortalidade" onPress={() => router.push(`/mortality-control?id=${id}`)}/>
                    <MenuButton title="Venda de Ovos" onPress={() => router.push(`/egg-sales-control?id=${id}`)}/>
                </View>
            </ScrollView>
        </SafeAreaView>
    );
}

const MenuButton = ({ title, onPress }: { title: string, onPress?: () => void }) => (
    <TouchableOpacity 
        className="bg-[#8B5CF6] w-full py-4 rounded-full items-center shadow-sm shadow-purple-900 border-b-4 border-purple-700 active:border-b-0 active:mt-1"
        onPress={onPress}
    >
        <Text className="text-white font-bold text-base text-center">
            {title}
        </Text>
    </TouchableOpacity>
);