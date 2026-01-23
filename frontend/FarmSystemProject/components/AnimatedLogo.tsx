import React, { useEffect } from 'react';
import { StyleSheet, View } from 'react-native';
import Svg, { Path, G } from 'react-native-svg';
import Animated, { useAnimatedProps, useSharedValue, withTiming, Easing, runOnJS } from 'react-native-reanimated';

const AnimatedPath = Animated.createAnimatedComponent(Path);

interface AnimatedLogoProps {
    onAnimationComplete?: () => void;
}

export default function AnimatedLogo({ onAnimationComplete }: AnimatedLogoProps) {
    // Approximate length of the path. 
    // You can find this exactly via path.getTotalLength() in a web api, or trial and error.
    // Given the coordinates (up to ~800-1000 units), 5000 is a safe upper bound for a full stroke.
    // The path data is scaled by 0.1 so the visual size is smaller, but the "units" in 'd' are large.
    // The path data provided: M745 1646 c-71 -22...
    // The scale is 0.1. So the effective length in the viewbox 170x175 is smaller, but dashArray applies to path units.
    // Let's stick to a large number to ensure it covers everything.
    const pathLength = 10000;

    const progress = useSharedValue(pathLength);

    const animatedProps = useAnimatedProps(() => ({
        strokeDashoffset: progress.value,
    }));

    useEffect(() => {
        progress.value = withTiming(0, {
            duration: 3000, 
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