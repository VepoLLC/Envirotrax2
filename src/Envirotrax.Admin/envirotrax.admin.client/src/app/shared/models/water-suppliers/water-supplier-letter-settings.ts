import { BackflowExpiredType, BackflowExpiringType, BackflowNonCompliantType } from './backflow-settings';
import { CsiImpendingType, CsiNonCompliantType, CsiPastDueType } from './csi-settings';

export class WaterSupplierLetterSettings {
    id?: number;
    name?: string;
    parentId?: number | null;

    expiringNotice1?: BackflowExpiringType | null;
    expiringNotice2?: BackflowExpiringType | null;
    expiredNotice1?: BackflowExpiredType | null;
    expiredNotice2?: BackflowExpiredType | null;
    backflowNonCompliant1?: BackflowNonCompliantType | null;
    backflowNonCompliant2?: BackflowNonCompliantType | null;
    backflowNonCompliantPrint1?: boolean | null;
    backflowNonCompliantPrint2?: boolean | null;
    unknownAssembliesNotice?: boolean | null;

    impendingNotice1?: CsiImpendingType | null;
    impendingNotice2?: CsiImpendingType | null;
    pastDueNotice1?: CsiPastDueType | null;
    pastDueNotice2?: CsiPastDueType | null;
    csiNonCompliant1?: CsiNonCompliantType | null;
    csiNonCompliant2?: CsiNonCompliantType | null;
    csiNonCompliantPrint1?: boolean | null;
    csiNonCompliantPrint2?: boolean | null;
}

export interface WaterSupplierLetterSettingsRow extends WaterSupplierLetterSettings {
    isSelected: boolean;
    hasLettersForGeneration: boolean;
}
