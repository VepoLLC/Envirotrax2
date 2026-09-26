import { CreditCardPayment } from "../payments/credit-card-payment";
import { BackflowTest } from "./backflow-test";

export interface BackflowCheckoutRequest {
    transactionId: string;
    tests: BackflowCheckoutTest[];
    expectedTotal: number;
    expectedCardCharge: number;
    card?: CreditCardPayment;
}

export interface BackflowCheckoutTest {
    id: number;
    emailPdf: boolean;
}

export interface BackflowCheckoutReceipt {
    transactionId: string;
    transactionDate: string;
    amount: number;
    balanceAdjustment: number;
    cardCharge: number;
    nameOnCard?: string;
    cardNumber?: string;
    tests: BackflowTest[];
    emailResults: BackflowCheckoutEmailResult[];
}

export interface BackflowCheckoutEmailResult {
    description: string;
    isSent: boolean;
}
