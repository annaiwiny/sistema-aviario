import React, { useEffect, useState } from 'react';
import { View, Text, TouchableOpacity, ScrollView, ActivityIndicator } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import Svg, { G, Path, Circle, Text as SvgText } from 'react-native-svg';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_URL } from '@/constants/Api';

interface LotDetails {
    id: number;
    accommodationDate: string;
    items: { race: string, quantity: number }[];
    farmId: number;
}

export default function LotDetailsScreen() {
    const { id } = useLocalSearchParams();
    const router = useRouter();
    const [loading, setLoading] = useState(true);
    const [lot, setLot] = useState<LotDetails | null>(null);


    const productionData = {
        producing: 50,
        notProducing: 150,
        percentage: 25
    };

    useEffect(() => {
        const fetchLot = async () => {
            try {
                const token = await AsyncStorage.getItem('userToken');
                const response = await fetch(`${API_URL}/api/Lot/${id}`, {
                    headers: { 'Authorization': `Bearer ${token}` }
                });
                if (response.ok) {
                    const data = await response.json();
                    setLot(data);
                }
            } catch (error) {
                console.error(error);
            } finally {
                setLoading(false);
            }
        };

        if (id) fetchLot();
    }, [id]);

    const PieChart = () => {
        // Simple Pie Chart: 75% Purple, 25% Green
        // 25% is 90 degrees.
        // Center 100,100. Radius 80.
        // Start (100, 20).
        // End (180, 100).
        // Large Arc Flag 0. Sweep Flag 1.

        // Green Slice (25%)
        const greenPath = `
            M 100 100
            L 100 20
            A 80 80 0 0 1 180 100
            Z
        `;

        // Purple Slice (75% - the rest)
        const purplePath = `
            M 100 100
            L 180 100
            A 80 80 0 1 1 100 20
            Z
        `;

        return (
            <View className="items-center justify-center my-4">
                <Svg height="200" width="200" viewBox="0 0 200 200">
                    {/* Purple Segment (Not Producing) */}
                    <Path d={purplePath} fill="#8B5CF6" />
                    <SvgText fill="white" fontSize="16" fontWeight="bold" x="70" y="120" textAnchor="middle">
                        {productionData.notProducing}
                    </SvgText>

                    {/* Green Segment (Producing) */}
                    <Path d={greenPath} fill="#D1FAE5" />
                    <SvgText fill="#10B981" fontSize="16" fontWeight="bold" x="140" y="70" textAnchor="middle">
                        {productionData.producing}
                    </SvgText>
                </Svg>
                <Text className="font-bold text-black mt-2">POSTURA: {productionData.percentage}%</Text>
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
            {/* Header - Moved outside ScrollView */}
            <View className="flex-row items-center justify-between px-6 py-4">
                <TouchableOpacity
                    className="flex-row items-center z-10 p-2 -ml-2"
                    hitSlop={{ top: 20, bottom: 20, left: 20, right: 20 }}
                    onPress={() => {
                        if (router.canGoBack()) {
                            router.back();
                        } else {
                            router.replace('/dashboard');
                        }
                    }}
                >
                    <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                    <Text className="text-[#8B5CF6] font-bold text-lg ml-1">voltar</Text>
                </TouchableOpacity>

                <View className="flex-row items-center">
                    <Text className="text-2xl font-bold text-black uppercase mr-2">
                        LOTE {String(lot?.id || id).padStart(2, '0')}
                    </Text>
                    <TouchableOpacity>
                        <Ionicons name="pencil" size={20} color="#8B5CF6" />
                    </TouchableOpacity>
                </View>
                <View style={{ width: 40 }} />
            </View>

            <ScrollView contentContainerStyle={{ flexGrow: 1, paddingHorizontal: 24, paddingBottom: 24 }}>

                {/* Chart Section */}
                <PieChart />

                {/* Status Bar */}
                <View className="bg-[#D1FAE5] rounded-full py-3 px-6 mb-8 items-center justify-center">
                    <Text className="text-green-800 font-bold uppercase text-xs text-center">
                        {productionData.producing} GALINHAS ESTÃO COLOCANDO OVO
                    </Text>
                </View>

                {/* Action Menu */}
                <View className="bg-[#D1FAE5] rounded-3xl p-6 py-10 space-y-4 gap-4">

                    <MenuButton title="Coleta Diária de Ovos" />
                    <MenuButton title="Controle de alimentação" />
                    <MenuButton title="Vacinação" />
                    <MenuButton title="Controle de Mortalidade" />
                    <MenuButton title="Venda de Ovos" />
                    <MenuButton title="Histórico de Produção" />

                </View>

            </ScrollView>
        </SafeAreaView>
    );
}

const MenuButton = ({ title }: { title: string }) => (
    <TouchableOpacity className="bg-[#8B5CF6] w-full py-4 rounded-full items-center shadow-sm shadow-purple-900 border-b-4 border-purple-700 active:border-b-0 active:mt-1">
        <Text className="text-white font-bold text-base text-center">
            {title}
        </Text>
    </TouchableOpacity>
);
