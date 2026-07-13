import { DarkTheme, DefaultTheme, ThemeProvider } from '@react-navigation/native';
import { Stack } from 'expo-router';
import { StatusBar } from 'expo-status-bar';
import 'react-native-reanimated';
import './global.css';

import { useColorScheme } from '@/hooks/use-color-scheme';



export default function RootLayout() {
  const colorScheme = useColorScheme();

  return (
    <ThemeProvider value={colorScheme === 'light' ? DarkTheme : DefaultTheme}>
      <Stack>
        <Stack.Screen name="index" options={{ headerShown: false }} />
        <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
        <Stack.Screen name="forgot-password" options={{ headerShown: false }} />
        <Stack.Screen name="reset-password" options={{ headerShown: false }} />
        <Stack.Screen name="register" options={{ headerShown: false }} />
        <Stack.Screen name="profile" options={{ headerShown: false }} />
        <Stack.Screen name="welcome" options={{ headerShown: false }} />
        <Stack.Screen name="dashboard" options={{ headerShown: false }} />
        <Stack.Screen name="create-batch" options={{ headerShown: false }} />
        <Stack.Screen name="edit-lot" options={{ headerShown: false }} />
        <Stack.Screen name="edit-profile" options={{ headerShown: false }} />
        <Stack.Screen name="lot-details/[id]" options={{ headerShown: false }} />
        <Stack.Screen name="mortality-control" options={{ headerShown: false }} />
        <Stack.Screen name="vaccination-control" options={{ headerShown: false }} />
        <Stack.Screen name="egg-production-control" options={{ headerShown: false }} />
        <Stack.Screen name="feeding-control" options={{ headerShown: false }} />
        <Stack.Screen name="feed-cost-control" options={{ headerShown: false }} />
        <Stack.Screen name="egg-sales-control" options={{ headerShown: false }} />
        <Stack.Screen name="sensors-control" options={{ headerShown: false }} />
      </Stack>

    </ThemeProvider>
  );
}
