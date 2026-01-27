import React from 'react';
import { View, Text, TouchableOpacity, ScrollView } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useRouter } from 'expo-router';
import Svg, { G, Path, Ellipse } from 'react-native-svg';
import { Ionicons } from '@expo/vector-icons';

export default function Profile() {
    const router = useRouter();

    const handleLogout = () => {
        // Implement logout logic here
        router.replace('/');
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <ScrollView contentContainerStyle={{ flexGrow: 1, paddingHorizontal: 24, paddingBottom: 24, paddingTop: 5 }}>

                {/* Header */}
                <View className="flex-row justify-between items-end mb-2 relative">
                    <View className="absolute left-0 right-0 bottom-2 items-center justify-center">
                        <Text className="text-purple-500 font-bold text-2xl uppercase tracking-widest">PERFIL</Text>
                    </View>
                    <TouchableOpacity
                        className="flex-row items-center z-10 mb-2"
                        onPress={() => router.back()}
                    >
                        <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                        <Text className="text-purple-500 font-bold text-lg ml-1">voltar</Text>
                    </TouchableOpacity>

                    <Svg width="40" height="50" viewBox="0 0 170 175">
                        <G transform="translate(0,175) scale(0.1,-0.1)" fill="#8B5CF6" stroke="none">
                            <Path d="M745 1646 c-71 -22 -123 -55 -190 -117 -85 -80 -124 -135 -189 -266 -132 -265 -178 -549 -120 -743 47 -161 176 -316 316 -384 351 -169 762 2 880 364 29 91 36 275 14 390 -22 120 -59 227 -120 355 -80 166 -151 259 -251 330 -106 76 -237 103 -340 71z m162 -187 c150 -56 315 -334 364 -614 18 -107 8 -249 -24 -319 -59 -129 -140 -203 -268 -246 -52 -18 -81 -21 -155 -18 -81 3 -100 8 -167 41 -156 77 -247 230 -247 416 0 201 115 498 247 641 92 99 169 130 250 99z" />
                        </G>
                    </Svg>
                </View>
                {/* Underline */}
                <View className="h-0.5 bg-purple-400 w-full mb-10" />

                {/* Info Card */}
                <View className="bg-green-100 border-2 border-purple-400 rounded-2xl p-6 mb-10 shadow-sm">
                    <Text className="text-black text-xl font-medium text-center mb-6">AVIÁRIO CAIPIRA</Text>

                    <View className="space-y-3">
                        <Text className="text-black text-base">
                            <Text className="font-bold">EMAIL: </Text>joão@gmail.com
                        </Text>
                        <Text className="text-black text-base">
                            <Text className="font-bold">CPF: </Text>000.000.999-00
                        </Text>
                        <Text className="text-black text-base">
                            <Text className="font-bold">ESTADO: </Text>Ceará
                        </Text>
                        <Text className="text-black text-base">
                            <Text className="font-bold">CIDADE: </Text>Fortaleza
                        </Text>
                        <Text className="text-black text-base">
                            <Text className="font-bold">TELEFONE: </Text>(88) 9 9999-9999
                        </Text>
                    </View>
                </View>

                {/* Action Buttons */}
                <View className="mt-auto">
                    <View className="flex-row justify-between mb-4">
                        <TouchableOpacity
                            className="bg-purple-500 py-3 px-4 rounded-xl shadow-md w-[48%] items-center"
                            onPress={() => router.push('/edit-profile')}
                        >
                            <Text className="text-white font-bold text-base">Editar Perfil</Text>
                        </TouchableOpacity>
                        <TouchableOpacity className="bg-purple-500 py-3 px-4 rounded-xl shadow-md w-[48%] items-center">
                            <Text className="text-white font-bold text-base">Editar Acesso</Text>
                        </TouchableOpacity>
                    </View>

                    <View className="items-center">
                        <TouchableOpacity
                            className="bg-red-600 py-3 px-10 rounded-xl shadow-md items-center w-40"
                            onPress={handleLogout}
                        >
                            <Text className="text-white font-bold text-lg">Sair</Text>
                        </TouchableOpacity>
                    </View>
                </View>

            </ScrollView>
        </SafeAreaView>
    );
}
