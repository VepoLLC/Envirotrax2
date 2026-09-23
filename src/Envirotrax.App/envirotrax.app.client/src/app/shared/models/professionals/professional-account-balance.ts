import { CreditCardPayment } from "../payments/credit-card-payment";

export interface ProfessionalAccountBalance extends CreditCardPayment {
    transactionId: string;
    amountToAdd: number;
}
