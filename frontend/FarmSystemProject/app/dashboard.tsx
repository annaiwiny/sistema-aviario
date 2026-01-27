import React from 'react';
import { View, Text, TouchableOpacity, ScrollView } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { FontAwesome } from '@expo/vector-icons';
import { StatusBar } from 'expo-status-bar';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { useRouter } from 'expo-router';
import { useFocusEffect } from '@react-navigation/native';

import { Ionicons, MaterialCommunityIcons } from '@expo/vector-icons';
import { Modal, TouchableWithoutFeedback } from 'react-native';

export default function DashboardScreen() {
    const [aviaryName, setAviaryName] = React.useState('Aviário Caipira');
    const [modalVisible, setModalVisible] = React.useState(false);
    const router = useRouter();

    const notifications = [
        { id: 1, title: 'Alta Taxa de Mortalidade', time: '14:24', desc: 'Lote 01 ultrapassou 99%', icon: 'alert', type: 'alert' },
        { id: 2, title: 'Início de Postura', time: '14:24', desc: 'Lote 01 deu inicio a postura de ovos', icon: 'chicken', type: 'info' },
        { id: 3, title: 'Alta Taxa de Mortalidade', time: '14:24', desc: 'Lote 01 ultrapassou 99%', icon: 'basket', type: 'production' },
        { id: 4, title: 'Alta Taxa de Mortalidade', time: '14:24', desc: 'Lote 99 ultrapassou 99%', icon: 'alert', type: 'alert' },
        { id: 5, title: 'Alta Taxa de Mortalidade', time: '14:24', desc: 'Lote 01 ultrapassou 99%', icon: 'basket', type: 'production' },
    ];

    useFocusEffect(
        React.useCallback(() => {
            const loadAviaryName = async () => {
                const name = await AsyncStorage.getItem('aviaryName');
                if (name) {
                    setAviaryName(name);
                }
            };
            loadAviaryName();
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
                    <TouchableOpacity
                        className="bg-purple-500 rounded-xl p-8 mb-4 shadow-lg shadow-purple-200 items-center justify-center h-32"
                        activeOpacity={0.8}
                    >
                        <Text className="text-white text-xl font-bold tracking-widest uppercase">
                            LOTE 01
                        </Text>
                    </TouchableOpacity>
                </ScrollView>

                {/* Floating Action Button */}
                <View className="absolute bottom-8 left-0 right-0 items-center">
                    <TouchableOpacity
                        className="bg-purple-500 w-16 h-16 rounded-full items-center justify-center shadow-lg shadow-purple-300"
                        activeOpacity={0.8}
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
