import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert, ScrollView, ActivityIndicator, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Link, useRouter } from 'expo-router';
import Svg, { G, Path } from 'react-native-svg';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_URL } from '@/constants/Api';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const router = useRouter();

    const handleLogin = async () => {
        if (!email || !password) {
            Alert.alert('Erro', 'Por favor, preencha todos os campos.');
            return;
        }

        setIsLoading(true);

        try {
            const response = await fetch(`${API_URL}/api/Auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    email,
                    password
                }),
            });

            const data = await response.json();

            if (response.ok) {
                // Salvar token e dados do usuário
                await AsyncStorage.setItem('userToken', data.token);
                await AsyncStorage.setItem('userData', JSON.stringify(data.user));

                // Verificar se já existe configuração inicial (Ex: Nome do Aviário)
                const aviaryName = await AsyncStorage.getItem('aviaryName');
                const nextRoute = aviaryName ? '/dashboard' : '/welcome';

                // Redirecionar
                if (Platform.OS === 'web') {
                    // Pequeno delay para garantir que o estado de loading não quebre o DOM na transição
                    setTimeout(() => router.replace(nextRoute), 100);
                } else {
                    router.replace(nextRoute);
                }
            } else {
                const errorMessage = data.message || 'E-mail ou senha inválidos.';
                Alert.alert('Erro', errorMessage);
            }

        } catch (error) {
            console.error('Login error:', error);
            Alert.alert('Erro', 'Não foi possível conectar ao servidor.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <ScrollView
                contentContainerStyle={{ flexGrow: 1, justifyContent: 'center', padding: 24 }}
                showsVerticalScrollIndicator={false}
            >
                {/* Logo Section */}
                <View className="items-center mb-8">
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

                {/* Form Section */}
                <View className="w-full">
                    <View className="mb-6">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            E-mail <Text className="text-red-500">*</Text>
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="joão@gmail.com"
                            value={email}
                            onChangeText={setEmail}
                            keyboardType="email-address"
                            autoCapitalize="none"
                        />
                    </View>

                    <View className="mb-8">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            Senha <Text className="text-red-500">*</Text>
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="*************"
                            value={password}
                            onChangeText={setPassword}
                            secureTextEntry
                        />
                        <Link href="/forgot-password" asChild>
                            <TouchableOpacity>
                                <Text className="text-blue-600 font-bold mt-2 ml-1">Esqueceu a senha?</Text>
                            </TouchableOpacity>
                        </Link>
                    </View>
                </View>

                {/* Buttons Section */}

                <View className="mt-4 justify-center items-center">
                    <TouchableOpacity
                        className="bg-purple-500 py-6 w-60 rounded-3xl items-center shadow-md shadow-purple-200 mb-4 justify-center relative"
                        onPress={handleLogin}
                        disabled={isLoading}
                    >
                        <View style={{ opacity: isLoading ? 0 : 1 }}>
                            <Text className="text-white text-xl font-bold">Entrar</Text>
                        </View>

                        {isLoading && (
                            <View className="absolute inset-0 justify-center items-center">
                                <ActivityIndicator color="#FFF" />
                            </View>
                        )}
                    </TouchableOpacity>


                    <Link href="/register" asChild>
                        <TouchableOpacity className="bg-green-200 py-3 rounded-3xl items-center w-40">
                            <Text className="text-green-900 text-lg font-semibold">Cadastre-se</Text>
                        </TouchableOpacity>
                    </Link>

                </View>
            </ScrollView>
        </SafeAreaView>
    );
};

export default Login;
