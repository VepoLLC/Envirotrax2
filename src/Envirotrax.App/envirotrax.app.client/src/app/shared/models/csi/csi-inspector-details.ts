import { ProfessionalUser } from '../professionals/professional-user';
import { ProfessionalWaterSupplier } from '../professionals/professional-water-supplier';
import { ProfessionalUserLicense } from '../professionals/licenses/professional-user-license';
import { State } from '../lookup/state';

export interface CsiInspectorDetails {
    id?: number;
    name?: string;
    companyEmail?: string;
    address?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    phoneNumber?: string;
    faxNumber?: string;
    createdTime?: string;
    waterSuppliers?: ProfessionalWaterSupplier[];
    subAccounts?: ProfessionalUser[];
    licenses?: ProfessionalUserLicense[];
}
