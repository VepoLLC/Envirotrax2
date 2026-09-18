import { InsuranceStatus, InsuranceValidation } from '../models/professionals/insurance-validation';

export interface InsuranceStatusDisplay {
    message: string;
    valid: boolean;
}

const currencyFormatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 0
});

// Shared by the three submission screens so the status->message mapping lives in one place. The
// `default` branch covers InsuranceStatus.NotFound and any value the client doesn't recognize, and
// fails closed (valid: false) rather than silently letting an unmapped status through.
export function describeInsuranceStatus(validation: InsuranceValidation | undefined): InsuranceStatusDisplay {
    if (!validation) {
        return { message: 'No insurance policy found', valid: false };
    }

    switch (validation.status) {
        case InsuranceStatus.NotRequired:
            return { message: 'Insurance not required', valid: true };
        case InsuranceStatus.Valid:
            return { message: 'Insurance policy valid', valid: true };
        case InsuranceStatus.AwaitingValidation:
            return { message: 'Insurance policy awaiting validation...', valid: false };
        case InsuranceStatus.Expired:
            return { message: 'Expired insurance policy', valid: false };
        case InsuranceStatus.InsufficientCoverage:
            return { message: `Requires ${currencyFormatter.format(validation.requiredCoverage)} in insurance coverage`, valid: false };
        default:
            return { message: 'No insurance policy found', valid: false };
    }
}
