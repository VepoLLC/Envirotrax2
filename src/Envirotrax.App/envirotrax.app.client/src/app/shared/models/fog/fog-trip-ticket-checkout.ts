import { CheckoutItem, ProfessionalCheckoutReceipt, ProfessionalCheckoutRequest } from "../payments/professional-checkout";
import { FogTripTicket } from "./fog-trip-ticket";

export interface FogTripTicketCheckoutRequest extends ProfessionalCheckoutRequest {
    tickets: CheckoutItem[];
}

export interface FogTripTicketCheckoutReceipt extends ProfessionalCheckoutReceipt {
    tickets: FogTripTicket[];
}
