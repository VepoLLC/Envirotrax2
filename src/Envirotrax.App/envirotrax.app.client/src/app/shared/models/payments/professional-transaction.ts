export enum ProfessionalTransactionType {
    BackflowTestPayment = 4,
    CsiInspection = 6,
    BalanceAdjustment = 7,
    FogTripTicketPayment = 8,
    FogInspectionPayment = 9
}

export const professionalTransactionTypeLabels: { [type: number]: string } = {
    [ProfessionalTransactionType.BackflowTestPayment]: 'Backflow Test Submission',
    [ProfessionalTransactionType.CsiInspection]: 'CSI Inspection Submission',
    [ProfessionalTransactionType.BalanceAdjustment]: 'Balance Adjustment',
    [ProfessionalTransactionType.FogTripTicketPayment]: 'Trip Ticket Submission',
    [ProfessionalTransactionType.FogInspectionPayment]: 'FOG Inspection Submission'
};

export interface ProfessionalTransaction {
    id: number;
    transactionDate: string;
    transactionId: string;
    user?: { id?: number; emailAddress?: string };
    transactionType: ProfessionalTransactionType;
    referenceDescription?: string;
    balanceAdjustment: number;
    cardCharge: number;
    amount: number;
}
