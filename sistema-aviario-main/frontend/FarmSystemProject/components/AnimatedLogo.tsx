import React, { useEffect } from 'react';
import { StyleSheet, View } from 'react-native';
import Svg, { Path, G } from 'react-native-svg';
import Animated, { useAnimatedProps, useSharedValue, withTiming, Easing, runOnJS } from 'react-native-reanimated';

const AnimatedPath = Animated.createAnimatedComponent(Path);

interface AnimatedLogoProps {
    onAnimationComplete?: () => void;
}

export default function AnimatedLogo({ onAnimationComplete }: AnimatedLogoProps) {
    
    const pathLength = 10000;

    const progress = useSharedValue(pathLength);

    const animatedProps = useAnimatedProps(() => ({
        strokeDashoffset: progress.value,
    }));

    useEffect(() => {
        progress.value = withTiming(0, {
            duration: 2000, 
            easing: Easing.inOut(Easing.ease),
        }, (finished) => {
            if (finished && onAnimationComplete) {
                runOnJS(onAnimationComplete)();
            }
        });
    }, []);

    return (
        <View style={styles.container}>
            <Svg width={200} height={200} viewBox="0 0 170 175"> 
                <G transform="translate(0, 175) scale(0.1, -0.1)">
                    <AnimatedPath
                        d="M745 1646 c-71 -22 -123 -55 -190 -117 -85 -80 -124 -135 -189 -266
            -132 -265 -178 -549 -120 -743 47 -161 176 -316 316 -384 351 -169 762 2 880
            364 29 91 36 275 14 390 -22 120 -59 227 -120 355 -80 166 -151 259 -251 330
            -106 76 -237 103 -340 71z m162 -187 c150 -56 315 -334 364 -614 18 -107 8
            -249 -24 -319 -59 -129 -140 -203 -268 -246 -52 -18 -81 -21 -155 -18 -81 3
            -100 8 -167 41 -156 77 -247 230 -247 416 0 201 115 498 247 641 92 99 169
            130 250 99z"
                        fill="transparent"
                        stroke="#8455f9" 
                        strokeWidth="50" 
                        strokeDasharray={pathLength}
                        animatedProps={animatedProps}
                    />
                </G>
            </Svg>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        justifyContent: 'center',
        alignItems: 'center',
    },
});