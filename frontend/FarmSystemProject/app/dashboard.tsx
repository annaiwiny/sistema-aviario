import React, { useState } from 'react';
import { View, Text, TouchableOpacity, ScrollView, Modal, TouchableWithoutFeedback, ActivityIndicator } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { FontAwesome, Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import { StatusBar } from 'expo-status-bar';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { useRouter } from 'expo-router';
import { useFocusEffect } from '@react-navigation/native';
import { API_URL } from '@/constants/Api';

interface Lot {
    id: number;
    accommodationDate: string;
    raceName: string;
    raceQuantity: number;
    farmId: number;
}

export default function DashboardScreen() {
    const [aviaryName, setAviaryName] = useState('Aviário Caipira');
    const [modalVisible, setModalVisible] = useState(false);

    const [lots, setLots] = useState<Lot[]>([]);
    const [loading, setLoading] = useState(true);

    const router = useRouter();

    const notifications = [
        { id: 1, title: 'Alta Taxa de Mortalidade', time: '14:24', desc: 'Lote 01 ultrapassou 99%', icon: 'alert', type: 'alert' },
        { id: 2, title: 'Início de Postura', time: '14:24', desc: 'Lote 01 deu inicio a postura de ovos', icon: 'chicken', type: 'info' },
    ];

    useFocusEffect(
        React.useCallback(() => {
            const loadData = async () => {
                try {
                    setLoading(true);
                    // 1. Carregar nome do aviário
                    const name = await AsyncStorage.getItem('aviaryName');
                    if (name) setAviaryName(name);

                    // 2. Carregar lotes da API
                    const token = await AsyncStorage.getItem('userToken');
                    if (token) {
                        const response = await fetch(`${API_URL}/api/Lot`, {
                            headers: { 'Authorization': `Bearer ${token}` }
                        });

                        if (response.ok) {
                            const data = await response.json();
                            setLots(data);
                        } else {
                            console.log('Erro ao buscar lotes:', response.status);
                        }
                    }
                } catch (error) {
                    console.error('Erro ao carregar dashboard', error);
                } finally {
                    setLoading(false);
                }
            };
            loadData();
        }, [])
    );

    const getIcon = (type: string) => {
        switch (type) {
            case 'alert':
                return <Ionicons name="alert-circle-outline" size={32} color="black" />;
            case 'info':
                return <MaterialCommunityIcons name="bird" size={32} color="black" />;
            case 'production':
                return <MaterialCommunityIcons name="basket" size={32} color="black" />;
            default:
                return <Ionicons name="notifications-outline" size={32} color="black" />;
        }
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <StatusBar style="dark" />
            <View className="flex-1 px-4 pt-4">
                {/* Header */}
                <View className="flex-row justify-between items-center mb-6">
                    <TouchableOpacity onPress={() => router.push('/profile')}>
                        <FontAwesome name="user-circle" size={40} color="#8B5CF6" />
                    </TouchableOpacity>

                    <Text className="text-xl font-bold text-black" numberOfLines={1}>
                        {aviaryName}
                    </Text>

                    <TouchableOpacity className="relative" onPress={() => setModalVisible(true)}>
                        <FontAwesome name="bell" size={32} color="#8B5CF6" />
                        <View className="absolute top-0 right-1 w-3.5 h-3.5 bg-red-500 rounded-full border-2 border-white" />
                    </TouchableOpacity>
                </View>

                {/* Content */}
                <ScrollView
                    className="flex-1"
                    contentContainerStyle={{ paddingBottom: 100 }}
                    showsVerticalScrollIndicator={false}
                >
                    {loading ? (
                        <ActivityIndicator size="large" color="#8B5CF6" className="mt-10" />
                    ) : (
                        lots.length === 0 ? (
                            <View className="items-center mt-10">
                                <Text className="text-gray-500 font-medium">Nenhum lote cadastrado.</Text>
                                <Text className="text-gray-400 text-sm mt-2">Toque em + para adicionar.</Text>
                            </View>
                        ) : (
                            lots.map((lot) => (
                                <TouchableOpacity
                                    key={lot.id}
                                    className="bg-purple-500 rounded-xl p-6 mb-4 shadow-lg shadow-purple-200 h-32 justify-between"
                                    activeOpacity={0.8}
                                >
                                    <View className="flex-row justify-between items-start">
                                        <Text className="text-white text-xl font-bold tracking-widest uppercase">
                                            LOTE {String(lot.id).padStart(2, '0')}
                                        </Text>
                                        <Text className="text-white/80 text-sm font-medium">
                                            {new Date(lot.accommodationDate).toLocaleDateString('pt-BR')}
                                        </Text>
                                    </View>

                                    <View>
                                        <Text className="text-white font-semibold text-lg">{lot.raceName}</Text>
                                        <Text className="text-white/80">{lot.raceQuantity} aves</Text>
                                    </View>
                                </TouchableOpacity>
                            ))
                        )
                    )}
                </ScrollView>

                {/* Floating Action Button */}
                <View className="absolute bottom-8 left-0 right-0 items-center">
                    <TouchableOpacity
                        className="bg-purple-500 w-16 h-16 rounded-full items-center justify-center shadow-lg shadow-purple-300"
                        activeOpacity={0.8}
                        onPress={() => router.push('/create-batch')}
                    >
                        <FontAwesome name="plus" size={24} color="white" />
                    </TouchableOpacity>
                </View>
            </View>

            {/* Notifications Modal */}
            <Modal
                animationType="fade"
                transparent={true}
                visible={modalVisible}
                onRequestClose={() => setModalVisible(false)}
            >
                <TouchableOpacity
                    style={{ flex: 1, backgroundColor: 'rgba(0,0,0,0.5)', justifyContent: 'center', alignItems: 'center' }}
                    activeOpacity={1}
                    onPress={() => setModalVisible(false)}
                >
                    <TouchableWithoutFeedback>
                        <View className="bg-gray-300 w-[90%] h-[85%] rounded-3xl p-6 shadow-2xl">
                            <Text className="text-3xl font-bold text-center mb-8 uppercase tracking-wider">NOTIFICAÇÕES</Text>

                            <ScrollView showsVerticalScrollIndicator={false}>
                                {notifications.map((item) => (
                                    <View key={item.id} className="bg-purple-300 p-4 rounded-xl mb-4 flex-row items-center shadow-sm border border-purple-400">
                                        <View className="flex-1 mr-2">
                                            <View className="flex-row items-baseline mb-1">
                                                <Text className="font-bold text-lg text-black mr-2">{item.title}</Text>
                                                <Text className="text-sm text-gray-800">{item.time}</Text>
                                            </View>
                                            <Text className="text-gray-900 font-medium">{item.desc}</Text>
                                        </View>
                                        <View>
                                            {getIcon(item.type)}
                                        </View>
                                    </View>
                                ))}
                            </ScrollView>

                            <Text className="text-center text-gray-600 font-semibold mt-4">Toque fora da aba para sair</Text>
                        </View>
                    </TouchableWithoutFeedback>
                </TouchableOpacity>
            </Modal>

        </SafeAreaView>
    );
}
