import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert, ScrollView } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useRouter } from 'expo-router';
import Svg, { Path, G } from 'react-native-svg';
import { Ionicons } from '@expo/vector-icons';

export default function ForgotPassword() {
    const router = useRouter();
    const [email, setEmail] = useState('');

    const handleSendLink = () => {
        if (email) {
            Alert.alert('Sucesso', 'Link de recuperação enviado para o seu e-mail!');
            router.back();
        } else {
            Alert.alert('Erro', 'Por favor, informe um e-mail válido.');
        }
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <ScrollView
                contentContainerStyle={{ flexGrow: 1, padding: 24 }}
                showsVerticalScrollIndicator={false}
            >
                {/* Header Navigation */}
                <TouchableOpacity
                    className="flex-row items-center mb-6 self-start"
                    onPress={() => router.back()}
                >
                    <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                    <Text className="text-purple-500 font-bold text-lg ml-1">voltar</Text>
                </TouchableOpacity>

                {/* Logo */}
                <View className="items-center mb-6">
                    <Svg width="50" height="70" viewBox="0 0 170 175">
                        <G transform="translate(0,175) scale(0.1,-0.1)" fill="#8B5CF6" stroke="none">
                            <Path d="M745 1646 c-71 -22 -123 -55 -190 -117 -85 -80 -124 -135 -189 -266 -132 -265 -178 -549 -120 -743 47 -161 176 -316 316 -384 351 -169 762 2 880 364 29 91 36 275 14 390 -22 120 -59 227 -120 355 -80 166 -151 259 -251 330 -106 76 -237 103 -340 71z m162 -187 c150 -56 315 -334 364 -614 18 -107 8 -249 -24 -319 -59 -129 -140 -203 -268 -246 -52 -18 -81 -21 -155 -18 -81 3 -100 8 -167 41 -156 77 -247 230 -247 416 0 201 115 498 247 641 92 99 169 130 250 99z" />
                        </G>
                    </Svg>
                </View>

                {/* Title */}
                <Text className="text-4xl font-bold text-black mb-2">
                    Redefinição{'\n'}de senha!
                </Text>

                {/* Separator Line */}
                <View className="h-0.5 bg-purple-300 w-full mb-4 shadow-sm" />

                {/* Description */}
                <Text className="text-gray-500 text-base mb-10 leading-6">
                    Informe um e-mail cadastrado e enviaremos um link para recuperação de sua senha.
                </Text>

                {/* Email Input */}
                <View className="mb-10">
                    <Text className="text-black font-bold mb-2 ml-1 text-base">
                        E-mail
                    </Text>
                    <TextInput
                        className="bg-gray-200 rounded-xl p-4 text-base text-gray-800 shadow-sm"
                        placeholder=""
                        value={email}
                        onChangeText={setEmail}
                        keyboardType="email-address"
                        autoCapitalize="none"
                    />
                </View>

                {/* Submit Button */}
                <TouchableOpacity
                    className="bg-purple-500 py-4 rounded-xl items-center shadow-md shadow-purple-200"
                    onPress={handleSendLink}
                >
                    <Text className="text-white text-lg font-bold">Enviar link de recuperação</Text>
                </TouchableOpacity>

            </ScrollView>
        </SafeAreaView>
    );
}
