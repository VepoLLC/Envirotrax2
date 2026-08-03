import { BackflowSettings } from './backflow-settings';
import { GeneralSettings } from './general-settings';
import { WaterSupplier } from './water-supplier';

export class WaterSupplierDetails {
    waterSupplier: WaterSupplier = {};
    generalSettings: GeneralSettings = {};
    backflowSettings: BackflowSettings = {};
}
