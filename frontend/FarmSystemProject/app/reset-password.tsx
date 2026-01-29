import React, { useState, useEffect } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert, ScrollView, ActivityIndicator } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useRouter, useLocalSearchParams } from 'expo-router'; // Importar Stack
import Svg, { Path, G } from 'react-native-svg';
import { Ionicons } from '@expo/vector-icons';
import { API_URL } from '@/constants/Api';
import SuccessModal from '@/components/SuccessModal';

export default function ResetPassword() {
    const router = useRouter();
    const { token } = useLocalSearchParams(); 

    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [showSuccessModal, setShowSuccessModal] = useState(false);

    useEffect(() => {
        if (!token) {
            Alert.alert('Atenção', 'Link de recuperação inválido ou expirado.');
        }
    }, [token]);

    const handleResetPassword = async () => {
        if (!token) {
            Alert.alert('Erro', 'Token de recuperação não encontrado.');
            return;
        }
        if (!newPassword || !confirmPassword) {
            Alert.alert('Erro', 'Por favor, preencha todos os campos.');
            return;
        }
        if (newPassword !== confirmPassword) {
            Alert.alert('Erro', 'As senhas não coincidem.');
            return;
        }

        setIsLoading(true);

        try {
            const rawToken = token.toString();
            const fixedToken = rawToken.replace(/ /g, '+'); 

            const response = await fetch(`${API_URL}/api/Auth/reset-password`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    token: fixedToken,
                    newPassword: newPassword
                }),
            });

            if (response.ok) {
                setShowSuccessModal(true);
            } else {
                const errorData = await response.json().catch(() => ({}));
                const msg = errorData.message || 'Não foi possível redefinir a senha.';
                Alert.alert('Erro', msg);
            }

        } catch (error) {
            console.error('Erro reset senha', error);
            Alert.alert('Erro', 'Falha na conexão com o servidor.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleSuccessClose = () => {
        setShowSuccessModal(false);
        router.replace('/'); 
    };

    return (
        <SafeAreaView className="flex-1 bg-white">
            <ScrollView
                contentContainerStyle={{ flexGrow: 1, padding: 24 }}
                showsVerticalScrollIndicator={false}
            >
                {/* Header Navigation */}
                <View className="flex-row items-center mb-6 relative mb-10">
                    <TouchableOpacity
                        className="flex-row items-center z-10"
                        onPress={() => router.replace('/')} // Força voltar para Login
                    >
                        <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                        <Text className="text-purple-500 font-bold text-lg ml-1">voltar</Text>
                    </TouchableOpacity>

                    <View className="absolute left-0 right-0 items-center">
                        <Svg width="50" height="70" viewBox="0 0 170 175">
                            <G transform="translate(0,175) scale(0.1,-0.1)" fill="#8B5CF6" stroke="none">
                                <Path d="M745 1646 c-71 -22 -123 -55 -190 -117 -85 -80 -124 -135 -189 -266 -132 -265 -178 -549 -120 -743 47 -161 176 -316 316 -384 351 -169 762 2 880 364 29 91 36 275 14 390 -22 120 -59 227 -120 355 -80 166 -151 259 -251 330 -106 76 -237 103 -340 71z m162 -187 c150 -56 315 -334 364 -614 18 -107 8 -249 -24 -319 -59 -129 -140 -203 -268 -246 -52 -18 -81 -21 -155 -18 -81 3 -100 8 -167 41 -156 77 -247 230 -247 416 0 201 115 498 247 641 92 99 169 130 250 99z" />
                            </G>
                        </Svg>
                    </View>
                </View>

                {/* Title */}
                <Text className="text-4xl font-bold text-black mb-2">
                    Redefinição{'\n'}de senha!
                </Text>

                {/* Separator Line */}
                <View className="h-0.5 bg-purple-300 w-full mb-4 shadow-sm" />

                {/* Description */}
                <Text className="text-gray-500 text-base mb-8 leading-6">
                    Defina sua nova senha e confirme-a!
                </Text>

                <View className="space-y-6 mb-10">
                    {/* Nova Senha */}
                    <View>
                        <Text className="text-black font-bold mb-2 ml-1 text-base">
                            Nova senha:
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-xl p-4 text-base text-gray-800 shadow-sm"
                            placeholder=""
                            value={newPassword}
                            onChangeText={setNewPassword}
                            secureTextEntry
                        />
                    </View>

                    {/* Confirmar Senha */}
                    <View>
                        <Text className="text-black font-bold mb-2 ml-1 text-base">
                            Confirme a nova senha:
                        </Text>
                        <TextInput
                            className="bg-gray-200 rounded-xl p-4 text-base text-gray-800 shadow-sm"
                            placeholder=""
                            value={confirmPassword}
                            onChangeText={setConfirmPassword}
                            secureTextEntry
                        />
                    </View>
                </View>

                {/* Submit Button */}
                <TouchableOpacity
                    className="bg-purple-500 py-4 rounded-xl items-center shadow-md shadow-purple-200"
                    onPress={handleResetPassword}
                    disabled={isLoading}
                >
                    {isLoading ? (
                        <ActivityIndicator color="white" />
                    ) : (
                        <Text className="text-white text-lg font-bold">Redefinir Senha</Text>
                    )}
                </TouchableOpacity>

            </ScrollView>

            <SuccessModal 
                visible={showSuccessModal} 
                onClose={handleSuccessClose}
                title="SENHA ALTERADA!"
                message="Sua senha foi redefinida com sucesso. Faça login novamente."
            />
        </SafeAreaView>
    );
}