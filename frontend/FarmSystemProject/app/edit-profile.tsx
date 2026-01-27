import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, KeyboardAvoidingView, Platform } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useRouter } from 'expo-router';
import Svg, { Ellipse } from 'react-native-svg';
import { Ionicons } from '@expo/vector-icons';

export default function EditProfile() {
    const router = useRouter();

    // State for form fields
    const [email, setEmail] = useState('joão@gmail.com');
    const [cpf, setCpf] = useState('000.000.999-00');
    const [state, setState] = useState('Ceará');
    const [city, setCity] = useState('Fortaleza');
    const [phone, setPhone] = useState('(99)99999-9999');

    const handleSave = () => {
        // Implement save logic here
        router.back();
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <KeyboardAvoidingView
             
                style={{ flex: 1 }}
            >
                <ScrollView contentContainerStyle={{ flexGrow: 1, padding: 24 }}>

                    {/* Header */}
                    <View className="flex-row justify-between items-end mb-2">
                        <View className="flex-row items-center mb-1">
                            <Text className="text-purple-500 font-bold text-2xl uppercase tracking-widest">
                                EDITAR PERFIL
                            </Text>
                        </View>

                        {/* Egg Icon */}
                        <Svg height="40" width="30" viewBox="0 0 100 140" className="mb-1">
                            <Ellipse
                                cx="50"
                                cy="70"
                                rx="45"
                                ry="65"
                                stroke="#8B5CF6"
                                strokeWidth="8"
                                fill="none"
                            />
                        </Svg>
                    </View>

                    {/* Underline */}
                    <View className="h-0.5 bg-purple-400 w-full mb-8" />

                    {/* Form Fields */}
                    <View className="space-y-4">
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
                    </View>

                    {/* Save Button */}
                    <View className="mt-auto pt-10 items-center">
                        <TouchableOpacity
                            className="bg-purple-500 py-4 px-12 rounded-2xl shadow-lg shadow-purple-200 w-64 items-center"
                            onPress={handleSave}
                        >
                            <Text className="text-white font-bold text-xl">Salvar</Text>
                        </TouchableOpacity>
                    </View>

                </ScrollView>
            </KeyboardAvoidingView>
        </SafeAreaView>
    );
}
