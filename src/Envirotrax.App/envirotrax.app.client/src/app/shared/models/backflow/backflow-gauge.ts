import { ProfessionalUser } from "../professionals/professional-user";

export interface BackflowGauge {
    id?: number;
    professionalId?: number;
    professionalUser?: ProfessionalUser | null;
    manufacturer?: string;
    model?: string;
    serialNumber?: string;
    lastCalibrationDate?: Date | string;
    isPortable?: boolean;
    filePath?: string;
    expirationType?: GaugeExpirationType;
}

export enum GaugeExpirationType {
    Valid,
    AboutToExpire,
    Expired
}
