export const MODULE_COLORS = {
    WATER_SUPPLIER: '#ff00ff',
    SITE: '#ffa500',
    CSI: '#0000ff',
    BACKFLOW: '#60e0c0',
    FOG: '#a52a2a',
    VALIDATION: '#ff0000',
    LICENSES: '#800080'
} as const;

export const DEFAULT_WINDOW_COLOR = '#e9ecef';

export interface ContrastClasses {
    text: string;
    button: string;
}

export const CONTRAST_CLASSES = {
    DARK: { text: 'text-black', button: 'text-black border-black' },
    LIGHT: { text: 'text-white', button: 'text-white border-white' }
} as const satisfies Record<string, ContrastClasses>;

const BRIGHTNESS_THRESHOLD = 128;

export function getContrastClasses(hex: string): ContrastClasses {
    const digits = hex.replace('#', '');
    const fullDigits = digits.length === 3
        ? digits.split('').map(digit => digit + digit).join('')
        : digits;

    const red = parseInt(fullDigits.substring(0, 2), 16);
    const green = parseInt(fullDigits.substring(2, 4), 16);
    const blue = parseInt(fullDigits.substring(4, 6), 16);

    const brightness = (red * 299 + green * 587 + blue * 114) / 1000;

    return brightness >= BRIGHTNESS_THRESHOLD ? CONTRAST_CLASSES.DARK : CONTRAST_CLASSES.LIGHT;
}
