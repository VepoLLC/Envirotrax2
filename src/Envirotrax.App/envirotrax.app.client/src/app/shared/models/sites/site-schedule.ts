import { ProfessionalType } from '../professionals/licenses/professional-user-license';

export interface SiteSchedule {
    id?: number;
    professionalType?: ProfessionalType;
    scheduleDate?: string;
    createdTime?: string;
}
