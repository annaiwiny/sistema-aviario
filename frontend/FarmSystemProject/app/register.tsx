import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert, ScrollView, ActivityIndicator, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useRouter } from 'expo-router';
import Svg, { Path, G } from 'react-native-svg';
import { Ionicons } from '@expo/vector-icons';
import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal';

export default function Register() {
    const router = useRouter();

    // Form states

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [cpf, setCpf] = useState('');
    const [state, setState] = useState('');
    const [city, setCity] = useState('');
    const [phone, setPhone] = useState('');
    const [birthDate, setBirthDate] = useState('2000-01-01'); // Valor padrão para evitar erro, ideal seria um DatePicker
    const [address, setAddress] = useState('');

    const [isLoading, setIsLoading] = useState(false);
    const [showSuccessModal, setShowSuccessModal] = useState(false);

    const handleRegister = async () => {
        if (!email || !password || !confirmPassword || !cpf || !state || !city || !phone) {
            Alert.alert('Erro', 'Por favor, preencha todos os campos obrigatórios.');
            return;
        }

        if (password !== confirmPassword) {
            Alert.alert('Erro', 'As senhas não coincidem.');
            return;
        }

        setIsLoading(true);

        try {
            const response = await fetch(`${API_URL}/api/User`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({

                    email,
                    password,
                    cpf,
                    state,
                    city,
                    phone,
                    birthDate: new Date(birthDate).toISOString(),
                    address: address || 'Endereço não informado'
                }),
            });

            const data = await response.json();

            if (response.ok) {
                setShowSuccessModal(true);
            } else {
                const errorMessage = data.message || data.title || 'Ocorreu um erro ao realizar o cadastro.';
                Alert.alert('Erro', errorMessage);
                console.error('Erro cadastro:', data);
            }

        } catch (error) {
            console.error('Erro de conexão:', error);
            Alert.alert('Erro', 'Não foi possível conectar ao servidor.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleSuccessClose = () => {
        setShowSuccessModal(false);
        router.replace('/(tabs)');
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <ScrollView
                contentContainerStyle={{ flexGrow: 1, padding: 24 }}
                showsVerticalScrollIndicator={false}
            >
                {/* Header Navigation */}
                <TouchableOpacity
                    className="flex-row items-center mb-0 self-start"
                    onPress={() => router.back()}
                >
                    <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                    <Text className="text-purple-500 font-bold text-lg ml-1">voltar</Text>
                </TouchableOpacity>

                {/* Logo */}
                <View className="items-center mb-6">
                    <Svg width="150" height="150" viewBox="0 0 118 174">
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

                {/* Form Fields */}
                <View className="w-full space-y-4">



                    {/* Email */}
                    <View className="mb-4">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            Email
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

                    {/* Senha */}
                    <View className="mb-4">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            Senha
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="*************"
                            value={password}
                            onChangeText={setPassword}
                            secureTextEntry
                        />
                    </View>

                    {/* Confirmar Senha */}
                    <View className="mb-4">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            Confirmar senha
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="*************"
                            value={confirmPassword}
                            onChangeText={setConfirmPassword}
                            secureTextEntry
                        />
                    </View>

                    {/* CPF */}
                    <View className="mb-4">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            CPF
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="000.000.000-00"
                            value={cpf}
                            onChangeText={setCpf}
                            keyboardType="numeric"
                        />
                    </View>

                    {/* Estado */}
                    <View className="mb-4">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            Estado
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="Ex: SP"
                            value={state}
                            onChangeText={setState}
                            maxLength={2}
                            autoCapitalize="characters"
                        />
                    </View>

                    {/* Cidade */}
                    <View className="mb-4">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            Cidade
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="Sua cidade"
                            value={city}
                            onChangeText={setCity}
                        />
                    </View>

                    {/* Endereço */}
                    <View className="mb-4">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            Endereço
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="Logradouro, número..."
                            value={address}
                            onChangeText={setAddress}
                        />
                    </View>

                    {/* Telefone */}
                    <View className="mb-8">
                        <Text className="text-black font-semibold mb-2 ml-1 text-base">
                            Telefone
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-lg p-4 text-base text-gray-800 shadow"
                            placeholder="(99)99999-9999"
                            value={phone}
                            onChangeText={setPhone}
                            keyboardType="phone-pad"
                        />
                    </View>

                </View>

                {/* Submit Button */}
                <View className="items-center mt-4 mb-8">
                    <TouchableOpacity
                        className="bg-purple-500 py-4 w-60 rounded-3xl items-center shadow-md shadow-purple-200 justify-center relative"
                        onPress={handleRegister}
                        disabled={isLoading}
                    >
                        <View style={{ opacity: isLoading ? 0 : 1 }}>
                            <Text className="text-white text-xl font-bold">Cadastrar</Text>
                        </View>

                        {isLoading && (
                            <View className="absolute inset-0 justify-center items-center">
                                <ActivityIndicator color="#FFF" />
                            </View>
                        )}
                    </TouchableOpacity>
                </View>

            </ScrollView>

            <SuccessModal visible={showSuccessModal} onClose={handleSuccessClose} />
        </SafeAreaView>
    );
}
