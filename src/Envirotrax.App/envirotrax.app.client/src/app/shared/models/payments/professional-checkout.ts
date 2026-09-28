import { CreditCardPayment } from "./credit-card-payment";

export interface ProfessionalCheckoutRequest {
    transactionId: string;
    expectedTotal: number;
    expectedCardCharge: number;
    card?: CreditCardPayment;
}

export interface CheckoutItem {
    id: number;
    emailPdf: boolean;
}

export interface ProfessionalCheckoutReceipt {
    transactionId: string;
    transactionDate: string;
    amount: number;
    balanceAdjustment: number;
    cardCharge: number;
    nameOnCard?: string;
    cardNumber?: string;
    emailResults: CheckoutEmailResult[];
}

export interface CheckoutEmailResult {
    description: string;
    isSent: boolean;
}
