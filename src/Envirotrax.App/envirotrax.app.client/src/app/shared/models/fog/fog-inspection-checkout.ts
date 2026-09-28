import { CheckoutItem, ProfessionalCheckoutReceipt, ProfessionalCheckoutRequest } from "../payments/professional-checkout";
import { FogInspection } from "./fog-inspection";

export interface FogInspectionCheckoutRequest extends ProfessionalCheckoutRequest {
    inspections: CheckoutItem[];
}

export interface FogInspectionCheckoutReceipt extends ProfessionalCheckoutReceipt {
    inspections: FogInspection[];
}
