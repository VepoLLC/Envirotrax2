import { CreditCardPayment } from "../payments/credit-card-payment";
import { BackflowTest } from "./backflow-test";

export interface BackflowCheckoutRequest {
    transactionId: string;
    tests: BackflowCheckoutTest[];
    expectedTotal: number;
    expectedCcCharge: number;
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
    ccCharge: number;
    ccNameOnCard?: string;
    ccNumber?: string;
    tests: BackflowTest[];
    emailResults: BackflowCheckoutEmailResult[];
}

export interface BackflowCheckoutEmailResult {
    description: string;
    isSent: boolean;
}
