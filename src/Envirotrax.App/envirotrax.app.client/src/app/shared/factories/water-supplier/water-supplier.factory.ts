import { WaterSupplier } from "../../models/water-suppliers/water-supplier";

export function createEmptyWaterSupplier(): WaterSupplier {
  return {
    letterReturn: {
      companyName: '',
      contactName: '',
      address: '',
      city: '',
      state: '',
      zipCode: ''
    },
    letterContact: {
      companyName: '',
      contactName: '',
      address: '',
      city: '',
      state: '',
      zipCode: '',
      phone: '',
      fax: '',
      email: ''
    }
  };
}
