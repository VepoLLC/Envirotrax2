import { Type } from '@angular/core';
import { PagedData, PageInfo, Query } from '@envirotrax/common-ui';
import { State } from '../lookup/state';
import { PropertyType } from '../sites/site';
import { WaterSupplier } from '../water-suppliers/water-supplier';

/**
 * Why a submission needs validation. Each value is the DTO member that stores the flag, so a reason is also
 * the column the search filters on.
 */
export enum SubmissionValidationReason {
    NewSite = 'validationNewSite',
    SiteInformationChanged = 'validationSiteInformationChanged',
    CreatedFromCsi = 'validationCreatedFromCsi',
    DeviceInformationChanged = 'validationDeviceInformationChanged',
    DeviceMoved = 'validationDeviceMoved',
    HazardTypeChanged = 'validationHazardTypeChanged',
    ReceiverInformationChanged = 'validationReceiverInformationChanged',
    UnknownSerialNumber = 'validationUnknownSerialNumber'
}

export enum SubmissionValidationTab {
    CsiInspections = 'csiInspections',
    BackflowTests = 'backflowTests',
    FogInspections = 'fogInspections',
    FogTripTickets = 'fogTripTickets'
}

/** A status or reason badge, as V1 draws it in the filter legend and in the result rows. */
export interface SubmissionValidationFlag {
    label: string;
    badgeText?: string;
    badgeIconClass?: string;
    badgeClass: string;
}

export interface SubmissionValidationReasonFlag extends SubmissionValidationFlag {
    reason: SubmissionValidationReason;
}

export interface SubmissionValidationInfoLine {
    label: string;
    text?: string;
    date?: string;
}

/** The members every validated record type (CSI, backflow, FOG inspection, trip ticket) shares. */
export type SubmissionValidationRecord = Partial<Record<SubmissionValidationReason, boolean>> & {
    id?: number;
    waterSupplier?: WaterSupplier;
    needsValidation?: boolean;
    validationOnHold?: boolean;
    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;
};

/** Grid row: display values are built once after each load so the templates only read plain fields. */
export interface SubmissionValidationRow {
    id?: number;
    waterSupplier?: WaterSupplier;
    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyAddress: string;
    propertyCityStateZip: string;
    validationOnHold: boolean;
    statusFlag: SubmissionValidationFlag;
    reasonFlags: SubmissionValidationReasonFlag[];
    infoLines: SubmissionValidationInfoLine[];
}

/** The Hold checkbox and the Clear action. Each record type's client service implements both. */
export interface SubmissionValidationActions {
    updateValidationHold(id: number, waterSupplierId: number, onHold: boolean): Promise<unknown>;
    updateNeedsValidation(id: number, waterSupplierId: number, needsValidation: boolean): Promise<unknown>;
}

/** Everything one result tab needs to load, show and act on its record type. */
export interface SubmissionValidationSource {
    infoCaption: string;
    reasons: SubmissionValidationReason[];
    detailsComponent?: Type<unknown>;
    actions: SubmissionValidationActions;
    load: (pageInfo: PageInfo, query: Query) => Promise<PagedData<SubmissionValidationRow>>;
}
