export interface CsiInspection {
    id?: number;
    site?: { id?: number; accountNumber?: string | null } | null;
    siteId?: number | null;
    waterSupplier?: { id?: number } | null;
    inspectorUser?: { id?: number } | null;
    inspectionDate?: string | null;
    submissionId?: string | null;

    // Property / Location
    propertyBusinessName?: string | null;
    propertyType?: number;
    propertyStreetNumber?: string | null;
    propertyStreetName?: string | null;
    propertyNumber?: string | null;
    propertyCity?: string | null;
    propertyState?: string | null;
    propertyZip?: string | null;

    // Mailing / Contact
    mailingCompanyName?: string | null;
    mailingContactName?: string | null;
    mailingStreetNumber?: string | null;
    mailingStreetName?: string | null;
    mailingNumber?: string | null;
    mailingCity?: string | null;
    mailingState?: string | null;
    mailingZip?: string | null;
    mailingPhoneNumber?: string | null;
    mailingEmailAddress?: string | null;

    // Inspector
    inspectorLicenseNumber?: string | null;
    inspectorLicenseType?: string | null;
    inspectorCompanyName?: string | null;
    inspectorJobTitle?: string | null;
    inspectorContactName?: string | null;
    inspectorAddress?: string | null;
    inspectorCity?: string | null;
    inspectorState?: string | null;
    inspectorZip?: string | null;
    inspectorWorkNumber?: string | null;
    inspectorCellNumber?: string | null;
    inspectorFaxNumber?: string | null;

    // Inspection criteria
    reasonForInspection?: number;
    compliance1?: boolean;
    compliance2?: boolean;
    compliance3?: boolean;
    compliance4?: boolean;
    compliance5?: boolean;
    compliance6?: boolean;

    // Material - service line
    materialServiceLineLead?: boolean;
    materialServiceLineCopper?: boolean;
    materialServiceLinePVC?: boolean;
    materialServiceLineOther?: boolean;
    materialServiceLineOtherDescription?: string | null;

    // Material - solder
    materialSolderLead?: boolean;
    materialSolderLeadFree?: boolean;
    materialSolderSolventWeld?: boolean;
    materialSolderOther?: boolean;
    materialSolderOtherDescription?: string | null;

    // Approval
    disapproved?: boolean;
    disapprovedReason?: string | null;

    // Additional infrastructure flags
    aiOssf?: boolean;
    aiWaterWell?: boolean;
    aiFireSystem?: boolean;
    aiFireSystem2?: boolean;
    aiGreaseTrap?: boolean;
    aiSandGrit?: boolean;
    aiReclaimedWater?: boolean;
    aiIrrigationSystem?: boolean;
    aiIrrigationSystem2?: boolean;
    aiHasDomesticPremisesIsolation?: boolean;
    aiRequiresDomesticPremisesIsolation?: boolean;

    comments?: string | null;

    // Validation
    needsValidation?: boolean;
    validationNewSite?: boolean;
    validationSiteInformationChanged?: boolean;

    // Payment
    transactionId?: string | null;
    transactionDate?: string | null;
    amount?: number;
    amountShare?: number;

    emailPdf?: boolean;
    includeWsAccountNumbers?: boolean;

    // Audit
    createdTime?: string;
    updatedTime?: string | null;
}
