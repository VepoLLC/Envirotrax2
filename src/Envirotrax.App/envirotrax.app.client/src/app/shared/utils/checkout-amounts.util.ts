export interface CheckoutAmounts {
    total: number;
    fromBalance: number;
    cardCharge: number;
}

export function calculateCheckoutAmounts(fees: number[], accountBalance: number): CheckoutAmounts {
    const total = roundToCents(fees.reduce((sum, fee) => sum + fee, 0));
    const availableBalance = Math.floor(accountBalance * 100) / 100;
    const fromBalance = Math.min(availableBalance, total);

    return { total, fromBalance, cardCharge: roundToCents(total - fromBalance) };
}

function roundToCents(amount: number): number {
    return Math.round(amount * 100) / 100;
}
