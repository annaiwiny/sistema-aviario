import { Alert, Platform } from 'react-native';

/**
 * Exibe um aviso ao usuário.
 *
 * No react-native-web o `Alert.alert` é uma função vazia (`static alert() {}`),
 * ou seja: na versão web do app nenhum erro ou aviso chegava na tela e o botão
 * parecia simplesmente não funcionar. Aqui caímos para o alerta do navegador.
 */
export const showAlert = (title: string, message?: string) => {
    if (Platform.OS === 'web') {
        if (typeof window !== 'undefined') {
            window.alert(message ? `${title}\n\n${message}` : title);
        }
        return;
    }

    Alert.alert(title, message);
};
