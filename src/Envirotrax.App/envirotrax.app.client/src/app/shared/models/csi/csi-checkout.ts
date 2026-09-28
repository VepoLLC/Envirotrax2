import { CheckoutItem, ProfessionalCheckoutReceipt, ProfessionalCheckoutRequest } from "../payments/professional-checkout";
import { CsiInspection } from "./csi-inspection";

export interface CsiCheckoutRequest extends ProfessionalCheckoutRequest {
    inspections: CheckoutItem[];
}

export interface CsiCheckoutReceipt extends ProfessionalCheckoutReceipt {
    inspections: CsiInspection[];
}
