import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert, ScrollView, ActivityIndicator } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useRouter } from 'expo-router';
import Svg, { G, Path } from 'react-native-svg';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal'; // Importando o Modal

export default function WelcomeScreen() {
    const [aviaryName, setAviaryName] = useState('');
    const router = useRouter();

    const [isLoading, setIsLoading] = useState(false);
    const [showSuccessModal, setShowSuccessModal] = useState(false);

    const handleRegister = async () => {
        if (!aviaryName.trim()) {
            Alert.alert('Erro', 'Por favor, informe o nome do aviário.');
            return;
        }

        setIsLoading(true);

        try {
            const token = await AsyncStorage.getItem('userToken');
            if (!token) {
                router.replace('/');
                return;
            }

            // POST para criar a granja
            const response = await fetch(`${API_URL}/api/Farm`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({ name: aviaryName })
            });

            if (response.ok) {
                const data = await response.json();
                
                // AJUSTE DE LÓGICA:
                // Salvamos APENAS o ID. O nome a Dashboard busca sozinha.
                if (data.id) {
                    await AsyncStorage.setItem('farmId', data.id.toString());
                }
                
                // Sucesso: Mostra o modal
                setShowSuccessModal(true);
            } else {
                const errorData = await response.json();
                Alert.alert('Erro', errorData.title || 'Falha ao criar aviário.');
            }

        } catch (error) {
            console.error(error);
            Alert.alert('Erro', 'Não foi possível conectar ao servidor.');
        } finally {
            setIsLoading(false);
        }
    };

    // Ao fechar o modal, vamos para o Dashboard
    const handleSuccessClose = () => {
        setShowSuccessModal(false);
        router.replace('/dashboard');
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <ScrollView
                contentContainerStyle={{ flexGrow: 1, alignItems: 'center', padding: 24, paddingTop: 60 }}
                showsVerticalScrollIndicator={false}
            >
                {/* Logo Section */}
                <View className="items-center mb-12">
                    <Svg width="150" height="220" viewBox="0 0 118 174">
                        <G transform="translate(0,174) scale(0.1,-0.1)" fill="#8B5CF6" stroke="none">
                            <Path d="M550 1587 c-64 -18 -111 -49 -165 -109 -129 -143 -222 -428 -197 -599 35 -237 225 -392 460 -376 176 11 317 116 378 281 29 78 26 265 -5 366 -48 157 -140 316 -213 372 -86 65 -176 88 -258 65z m155 -154 c62 -44 139 -166 175 -280 61 -188 50 -321 -36 -422 -108 -126 -316 -135 -433 -19 -86 86 -111 177 -86 319 33 193 131 365 240 424 40 21 89 13 140 -22z" />
                            <Path d="M805 361 c6 -51 4 -59 -12 -63 -35 -9 -43 2 -43 58 l0 54 -37 0 -38 0 0 -140 0 -140 38 0 37 0 0 55 c0 55 0 55 31 55 l31 0 -7 -60 -7 -60 40 0 39 0 -1 150 -1 150 -38 0 -39 0 7 -59z" />
                            <Path d="M254 328 c8 -96 -1 -94 -34 5 l-22 67 -39 0 -39 0 0 -135 0 -135 39 0 38 0 -5 73 c-5 83 5 82 36 -3 19 -53 21 -55 59 -58 l40 -3 -1 136 -1 135 -39 0 -39 0 7 -82z" />
                            <Path d="M348 270 l3 -140 37 0 37 0 0 140 0 140 -40 0 -40 0 3 -140z" />
                            <Path d="M582 335 c2 -41 0 -75 -3 -75 -4 0 -17 32 -29 70 l-23 70 -41 0 -41 0 0 -135 0 -135 40 0 40 0 -3 49 c-7 93 1 102 28 29 l25 -68 40 0 40 0 0 135 0 135 -38 0 -39 0 4 -75z" />
                            <Path d="M936 388 c-37 -33 -53 -105 -38 -165 16 -63 50 -93 106 -93 72 0 106 44 106 138 0 68 -18 115 -49 132 -35 18 -98 12 -125 -12z m94 -57 c15 -29 12 -104 -4 -127 -30 -40 -73 26 -60 91 11 55 44 73 64 36z" />
                        </G>
                    </Svg>
                </View>

                {/* Title Section */}
                <View className="w-full mb-10 ml-6 items-start ">
                    <Text className="text-4xl font-black text-black mb-2 text-center">BEM VINDO (A)</Text>
                    <Text className="text-[#8B5CF6] font-bold text-base text-center uppercase">
                        PARA COMEÇAR, CRIE SEU AVIÁRIO!
                    </Text>
                </View>

                {/* Form Section */}
                <View className="w-full px-4 mb-auto">
                    <Text className="text-black font-semibold mb-2 text-base">
                        Cadastrar aviário <Text className="text-red-500">*</Text>
                    </Text>
                    <TextInput
                        className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow-sm"
                        placeholder=""
                        value={aviaryName}
                        onChangeText={setAviaryName}
                    />
                </View>

                {/* Button Section */}
                <View className="w-full items-center mt-10 mb-8">
                    <TouchableOpacity
                        className="bg-purple-500 w-full py-4 rounded-full items-center shadow-lg shadow-purple-200"
                        onPress={handleRegister}
                        disabled={isLoading}
                    >
                        {isLoading ? (
                            <ActivityIndicator color="white" />
                        ) : (
                            <Text className="text-white text-lg font-bold">Cadastrar</Text>
                        )}
                    </TouchableOpacity>
                </View>

            </ScrollView>

            {/* Modal de Sucesso */}
            <SuccessModal 
                visible={showSuccessModal} 
                onClose={handleSuccessClose}
                title="SUCESSO!"
                message="AVIÁRIO CADASTRADO COM SUCESSO"
            />
        </SafeAreaView>
    );
}