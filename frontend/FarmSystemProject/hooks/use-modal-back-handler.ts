import { useCallback, useEffect, useRef } from 'react';
import { BackHandler, Platform } from 'react-native';

const isWeb = Platform.OS === 'web';

/**
 * Faz o botão de voltar (físico do Android ou do navegador) apenas fechar o modal
 * aberto, em vez de sair da tela atual e jogar o usuário de volta para o login.
 *
 * Retorna a função que o modal deve usar para fechar (botões, toque fora e
 * onRequestClose), para que o histórico do navegador fique consistente.
 *
 * Uso: const close = useModalBackHandler(visible, onClose);
 */
export function useModalBackHandler(visible: boolean, onClose: () => void) {
    // Guarda o callback em uma ref para o efeito depender apenas de "visible"
    const onCloseRef = useRef(onClose);
    onCloseRef.current = onClose;

    // Existe uma entrada extra de histórico criada por este modal? (somente web)
    const hasHistoryEntryRef = useRef(false);

    // Já pedimos o voltar do histórico e estamos aguardando o popstate?
    const waitingPopStateRef = useRef(false);

    useEffect(() => {
        if (!visible) return;

        // WEB: o Modal não intercepta o voltar do navegador, então criamos uma
        // entrada extra no histórico só para o modal "consumir" esse voltar.
        if (isWeb) {
            if (typeof window === 'undefined') return;

            window.history.pushState({ modalOpen: true }, '');
            hasHistoryEntryRef.current = true;

            const handlePopState = () => {
                // O navegador já consumiu a entrada extra
                hasHistoryEntryRef.current = false;
                waitingPopStateRef.current = false;
                onCloseRef.current();
            };

            window.addEventListener('popstate', handlePopState);

            return () => {
                window.removeEventListener('popstate', handlePopState);

                // Modal fechado sem passar pelo close() (a própria tela mudou o
                // estado): devolve a entrada extra para não sujar o histórico.
                if (hasHistoryEntryRef.current) {
                    hasHistoryEntryRef.current = false;
                    window.history.back();
                }
            };
        }

        // ANDROID: intercepta o botão físico e impede a navegação padrão
        const subscription = BackHandler.addEventListener('hardwareBackPress', () => {
            onCloseRef.current();
            return true;
        });

        return () => subscription.remove();
    }, [visible]);

    // Fechamento que o modal deve usar nos seus botões. Na web devolvemos a entrada
    // extra ao histórico primeiro e o onClose roda no popstate seguinte. Isso importa
    // porque alguns onClose navegam (router.back()): assim os dois "voltar" acontecem
    // em sequência, e não no mesmo instante.
    return useCallback(() => {
        if (isWeb && hasHistoryEntryRef.current) {
            hasHistoryEntryRef.current = false;
            waitingPopStateRef.current = true;
            window.history.back();

            // Rede de segurança: se o popstate não chegar, fecha assim mesmo
            setTimeout(() => {
                if (!waitingPopStateRef.current) return;

                waitingPopStateRef.current = false;
                onCloseRef.current();
            }, 300);

            return;
        }

        onCloseRef.current();
    }, []);
}
