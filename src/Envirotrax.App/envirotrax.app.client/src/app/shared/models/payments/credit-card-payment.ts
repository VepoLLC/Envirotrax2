import { State } from "../lookup/state";

export interface CreditCardPayment {
    dataDescriptor: string;
    dataValue: string;
    billingFirstName: string;
    billingLastName: string;
    billingAddress: string;
    billingCity: string;
    billingState: State;
    billingZipCode: string;
}
