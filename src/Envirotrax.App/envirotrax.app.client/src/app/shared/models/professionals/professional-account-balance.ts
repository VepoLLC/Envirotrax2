import { State } from "../lookup/state";

export interface ProfessionalAccountBalance {
    amountToAdd: number;
    dataDescriptor: string;
    dataValue: string;
    billingFirstName: string;
    billingLastName: string;
    billingAddress: string;
    billingCity: string;
    billingState: State;
    billingZipCode: string;
}
