import { ContrastClasses, DEFAULT_WINDOW_COLOR, getContrastClasses } from "../shared/models/module-colors";
import { WindowConfig } from "./window-config";

export interface WindowColors {
    background: string;
    contrast: ContrastClasses;
}

export function resolveWindowColors(config: WindowConfig<any>): WindowColors {
    const background = config.color ?? DEFAULT_WINDOW_COLOR;

    return {
        background,
        contrast: getContrastClasses(background)
    };
}
