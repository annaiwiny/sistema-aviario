import React from 'react';
import { View, Text, TouchableOpacity, Modal, ActivityIndicator, ScrollView } from 'react-native';
import { Ionicons } from '@expo/vector-icons';

interface ReportRow {
    label: string;
    value: string | number;
}

interface ReportResultModalProps {
    visible: boolean;
    onClose: () => void;
    title: string;
    dateDisplay: string;
    data: ReportRow[];
    onDownloadPdf: () => void;
    isDownloading?: boolean;
}

export default function ReportResultModal({
    visible,
    onClose,
    title,
    dateDisplay,
    data,
    onDownloadPdf,
    isDownloading = false
}: ReportResultModalProps) {

    return (
        <Modal
            animationType="fade"
            transparent={true}
            visible={visible}
            onRequestClose={onClose}
        >
            <View className="flex-1 bg-black/60 justify-center items-center px-4">
                <View className="bg-[#F3F4F6] w-full max-w-md rounded-3xl p-6 shadow-xl">
                    
                    {/* Header Voltar */}
                    <TouchableOpacity onPress={onClose} className="flex-row items-center mb-4 self-start">
                        <Ionicons name="chevron-back" size={24} color="#8B5CF6" />
                        <Text className="text-[#8B5CF6] font-bold text-base ml-1">voltar</Text>
                    </TouchableOpacity>

                    {/* Título */}
                    <Text className="text-black font-bold text-2xl text-center mb-6">
                        {title}
                    </Text>

                    {/* Tabela de Dados (Visual do Figma) */}
                    <View className="border border-gray-300 rounded-lg overflow-hidden bg-[#E5E7EB] mb-8">
                        <View className="bg-[#C4B5FD] py-2 items-center border-b border-gray-300">
                            <Text className="text-black font-bold text-lg">{dateDisplay}</Text>
                        </View>

                        {data.map((row, index) => (
                            <View 
                                key={index} 
                                className={`flex-row justify-between p-3 border-gray-300 ${index < data.length - 1 ? 'border-b' : ''}`}
                            >
                                <Text className="text-black font-bold text-base">{row.label}:</Text>
                                <Text className="text-black text-base">{row.value}</Text>
                            </View>
                        ))}
                    </View>

                    {/* Botão Baixar PDF */}
                    <TouchableOpacity
                        className="bg-[#8B5CF6] py-4 rounded-2xl items-center shadow-md shadow-purple-200 w-40 self-center"
                        onPress={onDownloadPdf}
                        disabled={isDownloading}
                    >
                        {isDownloading ? (
                            <ActivityIndicator color="white" />
                        ) : (
                            <Text className="text-white font-bold text-lg">Baixar PDF</Text>
                        )}
                    </TouchableOpacity>

                </View>
            </View>
        </Modal>
    );
}