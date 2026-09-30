import { CreditCardPayment } from "./credit-card-payment";

export interface CheckoutItem {
    id: number;
    emailPdf: boolean;
}

export interface ProfessionalCheckoutRequest {
    transactionId: string;
    items: CheckoutItem[];
    expectedTotal: number;
    expectedCardCharge: number;
    card?: CreditCardPayment;
}

export interface ProfessionalCheckoutReceipt<TItem> {
    transactionId: string;
    transactionDate: string;
    amount: number;
    balanceAdjustment: number;
    cardCharge: number;
    nameOnCard?: string;
    cardNumber?: string;
    emailResults: CheckoutEmailResult[];
    items: TItem[];
}

export interface CheckoutEmailResult {
    description: string;
    isSent: boolean;
}
