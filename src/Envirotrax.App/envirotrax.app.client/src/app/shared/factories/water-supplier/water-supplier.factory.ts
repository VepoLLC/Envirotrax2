import { WaterSupplier } from "../../models/water-suppliers/water-supplier";

export function createEmptyWaterSupplier(): WaterSupplier {
    return {
        letterAddress: {
            id: 0,
            companyName: '',
            contactName: '',
            address: '',
            city: '',
            stateId: 0,
            zipCode: ''
        },
        letterContact: {
            id: 0,
            companyName: '',
            contactName: '',
            address: '',
            city: '',
            stateId: 0,
            zipCode: '',
            phoneNumber: '',
            faxNumber: '',
            emailAddress: ''
        }
    };
}
