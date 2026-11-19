export interface CheckoutAmounts {
    total: number;
    fromBalance: number;
    cardCharge: number;
    isPaymentBypassed: boolean;
}

export function calculateCheckoutAmounts(fees: number[], accountBalance: number, isPaymentBypassed: boolean): CheckoutAmounts {
    const total = roundToCents(fees.reduce((sum, fee) => sum + fee, 0));

    if (total > 0 && isPaymentBypassed) {
        return { total, fromBalance: 0, cardCharge: 0, isPaymentBypassed: true };
    }

    const availableBalance = Math.floor(accountBalance * 100) / 100;
    const fromBalance = Math.min(availableBalance, total);

    return { total, fromBalance, cardCharge: roundToCents(total - fromBalance), isPaymentBypassed: false };
}

function roundToCents(amount: number): number {
    return Math.round(amount * 100) / 100;
}
