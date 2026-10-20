import { ReferencedProfessional } from '../professionals/professional';
import { ExpirationType } from '../professionals/licenses/professional-user-license';

export interface BackflowGauge {
    id?: number;
    professional?: ReferencedProfessional;
    manufacturer?: string;
    model?: string;
    serialNumber?: string;
    lastCalibrationDate?: string;
    isPortable?: boolean;
    filePath?: string;
    expirationType?: ExpirationType;
    createdTime?: string;
}
