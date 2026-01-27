import React, { useState, useEffect } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, KeyboardAvoidingView, Platform, Alert, ActivityIndicator } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useRouter } from 'expo-router';
import Svg, { G, Path } from 'react-native-svg';
import { Ionicons } from '@expo/vector-icons';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_URL } from '@/constants/Api';

export default function EditProfile() {
    const router = useRouter();

    // State for form fields
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [cpf, setCpf] = useState('');
    const [state, setState] = useState('');
    const [city, setCity] = useState('');
    const [phone, setPhone] = useState('');
    const [address, setAddress] = useState('');

    const [isLoading, setIsLoading] = useState(false);
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        loadUserData();
    }, []);

    const loadUserData = async () => {
        try {
            setIsLoading(true);
            const token = await AsyncStorage.getItem('userToken');

            if (token) {
                const response = await fetch(`${API_URL}/api/User/me`, {
                    headers: { 'Authorization': `Bearer ${token}` }
                });

                if (response.ok) {
                    const data = await response.json();
                    // Preencher campos com o que vier da API
                    setName(data.name || '');
                    setEmail(data.email || '');
                    setCpf(data.cpf || '');
                    setState(data.state || '');
                    setCity(data.city || '');
                    setPhone(data.phone || '');
                    setAddress(data.address || '');
                }
            }
        } catch (error) {
            console.error('Erro ao carregar dados do usuário', error);
            Alert.alert('Erro', 'Não foi possível carregar os dados.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleSave = async () => {
        try {
            setIsSaving(true);
            const token = await AsyncStorage.getItem('userToken');

            if (!token) {
                router.replace('/');
                return;
            }

            const payload = {
                name,
                email,
                cpf,
                state,
                city,
                address,
                phone
            };

            const response = await fetch(`${API_URL}/api/User/me`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                Alert.alert('Sucesso', 'Perfil atualizado com sucesso!', [
                    { text: 'OK', onPress: () => router.back() }
                ]);
            } else {
                const errorData = await response.json();
                Alert.alert('Erro', errorData.title || 'Falha ao atualizar perfil.');
            }

        } catch (error) {
            console.error('Erro ao salvar perfil', error);
            Alert.alert('Erro', 'Falha na conexão com o servidor.');
        } finally {
            setIsSaving(false);
        }
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <KeyboardAvoidingView
                behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
                style={{ flex: 1 }}
            >
                <ScrollView contentContainerStyle={{ flexGrow: 1, padding: 24, paddingTop: 0 }}>

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
                    <View className="h-0.5 bg-purple-400 w-full mb-8" />

                    {isLoading ? (
                        <ActivityIndicator size="large" color="#8B5CF6" className="mt-10" />
                    ) : (
                        <View className="space-y-4 gap-3">
                            {/* Nome */}
                            <View>
                                <Text className="text-black font-bold text-base mb-1 ml-1">Nome</Text>
                                <TextInput
                                    className="bg-gray-200 rounded-lg px-4 py-3 text-gray-700 text-base shadow-sm"
                                    value={name}
                                    onChangeText={setName}
                                />
                            </View>

                            <View>
                                <Text className="text-black font-bold text-base mb-1 ml-1">Email</Text>
                                <TextInput
                                    className="bg-gray-200 rounded-lg px-4 py-3 text-gray-700 text-base shadow-sm"
                                    value={email}
                                    onChangeText={setEmail}
                                    keyboardType="email-address"
                                />
                            </View>

                            <View>
                                <Text className="text-black font-bold text-base mb-1 ml-1">CPF</Text>
                                <TextInput
                                    className="bg-gray-200 rounded-lg px-4 py-3 text-gray-700 text-base shadow-sm"
                                    value={cpf}
                                    onChangeText={setCpf}
                                    keyboardType="numeric"
                                />
                            </View>

                            <View>
                                <Text className="text-black font-bold text-base mb-1 ml-1">Estado</Text>
                                <TextInput
                                    className="bg-gray-200 rounded-lg px-4 py-3 text-gray-700 text-base shadow-sm"
                                    value={state}
                                    onChangeText={setState}
                                />
                            </View>

                            <View>
                                <Text className="text-black font-bold text-base mb-1 ml-1">Cidade</Text>
                                <TextInput
                                    className="bg-gray-200 rounded-lg px-4 py-3 text-gray-700 text-base shadow-sm"
                                    value={city}
                                    onChangeText={setCity}
                                />
                            </View>

                            <View>
                                <Text className="text-black font-bold text-base mb-1 ml-1">Telefone</Text>
                                <TextInput
                                    className="bg-gray-200 rounded-lg px-4 py-3 text-gray-700 text-base shadow-sm"
                                    value={phone}
                                    onChangeText={setPhone}
                                    keyboardType="phone-pad"
                                />
                            </View>

                            {/* Save Button */}
                            <View className=" pt-10 items-center mb-10">
                                <TouchableOpacity
                                    className="bg-purple-500 py-4 px-12 rounded-2xl shadow-lg shadow-purple-200 w-64 items-center"
                                    onPress={handleSave}
                                    disabled={isSaving}
                                >
                                    {isSaving ? (
                                        <ActivityIndicator color="white" />
                                    ) : (
                                        <Text className="text-white font-bold text-xl">Salvar</Text>
                                    )}
                                </TouchableOpacity>
                            </View>
                        </View>
                    )}

                </ScrollView>
            </KeyboardAvoidingView>
        </SafeAreaView>
    );
}
