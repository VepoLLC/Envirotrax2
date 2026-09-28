import { CheckoutItem, ProfessionalCheckoutReceipt, ProfessionalCheckoutRequest } from "../payments/professional-checkout";
import { BackflowTest } from "./backflow-test";

export interface BackflowCheckoutRequest extends ProfessionalCheckoutRequest {
    tests: CheckoutItem[];
}

export interface BackflowCheckoutReceipt extends ProfessionalCheckoutReceipt {
    tests: BackflowTest[];
}
