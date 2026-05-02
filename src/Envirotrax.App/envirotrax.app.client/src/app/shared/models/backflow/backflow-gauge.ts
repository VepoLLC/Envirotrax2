export interface BackflowGauge {
    id?: number;
    professionalId?: number;
    manufacturer?: string;
    model?: string;
    serialNumber?: string;
    lastCalibrationDate?: Date | string;
    isPortable?: boolean;
    filePath?: string;
}
