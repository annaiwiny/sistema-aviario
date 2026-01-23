import React from 'react';
import { View, StyleSheet, Text } from 'react-native';
import { useRouter } from 'expo-router';
import AnimatedLogo from '@/components/AnimatedLogo';
import { StatusBar } from 'expo-status-bar';
import LogoText from '@/components/LogoText';
import { SafeAreaFrameContext } from 'react-native-safe-area-context';

export default function SplashScreen() {
    const router = useRouter();

    const handleAnimationComplete = () => {
        router.replace('/(tabs)');
    };

    return (
        <View style={styles.container}>
            <StatusBar style="dark" />
            <AnimatedLogo onAnimationComplete={handleAnimationComplete} />
            <LogoText/>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#ffffff',
        justifyContent: 'center',
        alignItems: 'center',
        gap: 20
    },

});
