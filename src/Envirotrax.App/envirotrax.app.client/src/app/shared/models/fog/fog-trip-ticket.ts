import { Site } from '../sites/site';
import { Professional } from '../professionals/professional';
import { ProfessionalUser } from '../professionals/professional-user';
import { State } from '../lookup/state';
import { WaterSupplier } from '../water-suppliers/water-supplier';
import { PropertyType } from '../../enums/property-type.enum';
import { FogVehicleCapacityType } from './fog-vehicle-enums';

export interface FogTripTicket {
    id: number;
    submissionId?: string;

    waterSupplier?: WaterSupplier | null;
    site?: Site | null;
    transporter?: ProfessionalUser | null;

    propertyBusinessName?: string;
    propertyType?: PropertyType;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State | null;
    propertyZip?: string;
    fogGeneratorPhoneNumber?: string;
    fogGeneratorEmailAddress?: string;
    fogGeneratorContactName?: string;

    professional?: Professional | null;
    transporter?: ProfessionalUser | null;
    transporterLicenseNumber?: string;
    transporterLicenseExpiration?: string;
    transporterCompanyName?: string;
    transporterContactName?: string;
    transporterAddress?: string;
    transporterCity?: string;
    transporterState?: string;
    transporterZip?: string;
    transporterWorkNumber?: string;
    transporterCellNumber?: string;
    transporterFaxNumber?: string;
    transporterEmailAddress?: string;
    transporterSignaturePath?: string;

    generatorContactName?: string;
    generatorSignaturePath?: string;
    generatorSignatureDate?: string;

    interceptorType?: string;
    interceptorOtherDescription?: string;
    interceptorCapacity?: number;
    interceptorCapacityType?: FogVehicleCapacityType;
    interceptorWasteRemovedAmount?: number;
    interceptorWasteRemovedType?: FogVehicleCapacityType;
    interceptorWasteRemovedAmountGallons?: number;
    interceptorWasteRemovedAmountCubicFeet?: number;
    interceptorWasteRemovedDate?: string;

    vehicleId?: number | null;
    vehicleLicensePlateNumber?: string;
    vehicleManufacturer?: string;
    vehicleYear?: number;
    vehicleCapacity?: number;
    vehicleCapacityType?: FogVehicleCapacityType;
    vehicleStickerNumber?: string;
    vehiclePermitNumber?: string;

    receiverDisposalSiteId?: number | null;
    receiverCompanyName?: string;
    receiverContactName?: string;
    receiverAddress?: string;
    receiverCity?: string;
    receiverState?: string;
    receiverZip?: string;
    receiverPhoneNumber?: string;
    receiverEmailAddress?: string;
    receiverRegistrationNumber?: string;
    receiverPermitNumber?: string;
    receiverWasteDeliveredDate?: string;
    receiverSignaturePath?: string;
    receiverSignatureDate?: string;

    pickupCompleted?: boolean;
    completed?: boolean;
    disapproved?: boolean;
    approvalDate?: string;
    approvedBy?: string;

    needsValidation?: boolean;
    validationOnHold?: boolean;
    validatedBy?: string;
    validationClearedDate?: string;
    validationLockedDate?: string;
    validationNewSite?: boolean;
    validationSiteInformationChanged?: boolean;
    validationReceiverInformationChanged?: boolean;
    validationNotes?: string;

    transactionId?: string;
    transactionDate?: string;
    amount?: number;
    amountShare?: number;
    wsPaidDate?: string;
    salesRepPaidDate?: string;

    emailPdf?: boolean;
    emailStatus?: number;
    comments?: string;

    createdTime?: string;
    updatedTime?: string;
}
