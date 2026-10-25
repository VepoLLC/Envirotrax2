import { State } from '../lookup/state';
import { PropertyType } from '../sites/site';
import { InterceptorCapacityType, ReferencedProfessionalUser, ReferencedWaterSupplier } from './fog-inspection';

export enum FogTripTicketStatus {
    PickupNotCompleted = 'pickupNotCompleted',
    PickupCompleted = 'pickupCompleted',
    TicketCompleted = 'ticketCompleted'
}

export enum FogTripTicketDateField {
    RecordCreationDate = 'createdTime',
    WasteRemovalDate = 'interceptorWasteRemovedDate',
    WasteDeliveredDate = 'receiverWasteDeliveredDate'
}

export interface ReferencedFogTripTicketSite {
    id?: number;
    accountNumber?: string;
}

export interface ReferencedProfessional {
    id?: number;
    name?: string;
}

export class FogTripTicket {
    id?: number;

    waterSupplier?: ReferencedWaterSupplier;
    site?: ReferencedFogTripTicketSite;

    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;

    professional?: ReferencedProfessional;
    transporter?: ReferencedProfessionalUser;
    transporterCompanyName?: string;
    transporterContactName?: string;
    transporterAddress?: string;
    transporterCity?: string;
    transporterState?: string;
    transporterZip?: string;
    transporterLicenseNumber?: string;
    transporterEmailAddress?: string;

    interceptorType?: string;
    interceptorOtherDescription?: string;
    interceptorCapacity?: number;
    interceptorCapacityType?: InterceptorCapacityType;
    interceptorWasteRemovedAmount?: number;
    interceptorWasteRemovedType?: InterceptorCapacityType;
    interceptorWasteRemovedDate?: string;

    receiverCompanyName?: string;
    receiverContactName?: string;
    receiverAddress?: string;
    receiverCity?: string;
    receiverState?: string;
    receiverZip?: string;
    receiverRegistrationNumber?: string;
    receiverWasteDeliveredDate?: string;

    pickupCompleted?: boolean;
    completed?: boolean;
    transactionId?: string;

    createdTime?: string;
}

export interface FogTripTicketRow extends FogTripTicket {
    propertyAddress: string;
    propertyCityStateZip: string;
    transporterCityStateZip: string;
    receiverCityStateZip: string;
    interceptorTypeDescription: string;
    interceptorCapacityDescription: string;
    wasteRemovedDescription: string;
}
