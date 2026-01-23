import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = () => {
        if (email && password) {
            // Logic for authentication would go here
            Alert.alert('Login', `Bem-vindo, ${email}!`);
        } else {
            Alert.alert('Erro', 'Por favor, preencha todos os campos.');
        }
    };

    return (
        <SafeAreaView className="flex-1 justify-center p-5 bg-gray-100">
            <Text className="text-3xl font-bold text-green-700 text-center mb-10">Farm System</Text>
            <View className="bg-white p-5 rounded-lg shadow-md">
                <TextInput
                    className="h-12 border border-gray-300 rounded-lg px-4 mb-5 text-base"
                    placeholder="Email"
                    value={email}
                    onChangeText={setEmail}
                    keyboardType="email-address"
                    autoCapitalize="none"
                />
                <TextInput
                    className="h-12 border border-gray-300 rounded-lg px-4 mb-5 text-base"
                    placeholder="Senha"
                    value={password}
                    onChangeText={setPassword}
                    secureTextEntry
                />
                <TouchableOpacity className="bg-green-700 py-4 rounded-lg items-center" onPress={handleLogin}>
                    <Text className="text-white text-lg font-bold">Entrar</Text>
                </TouchableOpacity>
            </View>
        </SafeAreaView>
    );
};

export default Login;
