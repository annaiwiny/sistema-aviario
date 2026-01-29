import React from 'react';
import { View, Text, Modal, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';

interface SuccessModalProps {
    visible: boolean;
    onClose: () => void;
    // Adicionamos estas duas props opcionais
    title?: string;
    message?: string;
    buttonText?: string;
}

export default function SuccessModal({ 
    visible, 
    onClose, 
    title = "SUCESSO!", // Valor padrão se não for informado
    message = "OPERAÇÃO REALIZADA COM SUCESSO", // Valor padrão genérico
    buttonText = "OK"
}: SuccessModalProps) {
    return (
        <Modal
            animationType="fade"
            transparent={true}
            visible={visible}
            onRequestClose={onClose}
        >
            <View className="flex-1 bg-black/50 justify-center items-center px-6">
                <View className="bg-white rounded-[32px] p-8 w-full max-w-[320px] items-center elevation-5 shadow-lg">
                    {/* Icon Circle */}
                    <View className="w-24 h-24 bg-[#6CC04A] rounded-full justify-center items-center mb-6">
                        <Ionicons name="checkmark" size={64} color="white" />
                    </View>

                    {/* Title (Agora dinâmico) */}
                    <Text className="text-3xl font-black text-black mb-3 text-center">
                        {title}
                    </Text>

                    {/* Message (Agora dinâmico) */}
                    <Text className="text-black text-center text-base font-normal mb-8 uppercase">
                        {message}
                    </Text>

                    {/* Button */}
                    <TouchableOpacity
                        className="bg-[#8B5CF6] w-48 py-3 rounded-full items-center shadow-lg shadow-purple-200"
                        onPress={onClose}
                    >
                        <Text className="text-white text-xl font-bold">{buttonText}</Text>
                    </TouchableOpacity>
                </View>
            </View>
        </Modal>
    );
}