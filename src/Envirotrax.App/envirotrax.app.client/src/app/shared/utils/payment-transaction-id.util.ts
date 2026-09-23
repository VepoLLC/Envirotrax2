const PAYMENT_TRANSACTION_ID_LENGTH = 20;

export function createPaymentTransactionId(): string {
    return crypto.randomUUID().replace(/-/g, '').substring(0, PAYMENT_TRANSACTION_ID_LENGTH);
}
