import { ProfessionalType } from "./licenses/professional-user-license";

export interface RegisteredProfessional {
    id?: number;
    companyName?: string;
    contactName?: string;
    registeredDate?: string;
    address?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    workNumber?: string;
    cellNumber?: string;
    faxNumber?: string;
    emailAddress?: string;
    websiteUrl?: string;
    hasFireLicense?: boolean;
}

export interface RegisteredProfessionalSupplier {
    id?: number;
    name?: string;
}

/**
 * Everything that differs between the four public directories. V1 kept the same wording in a set of
 * Select Case blocks in registrations.aspx.vb; here it is one table keyed by the URL segment the
 * marketing site links to (V1 used the numeric `at` query string parameter instead).
 */
export interface RegisteredProfessionalAccountType {
    slug: string;
    professionalType: ProfessionalType;
    name: string;
    pageTitle: string;
    resultsLabel: string;
    instructions: string;
    showFireLicense: boolean;
}

export const REGISTERED_PROFESSIONAL_ACCOUNT_TYPES: RegisteredProfessionalAccountType[] = [
    {
        slug: 'backflow-testers',
        professionalType: ProfessionalType.Bpat,
        name: 'Backflow Testers',
        pageTitle: 'Registered Backflow Tester Search',
        resultsLabel: 'Registered Backflow Testers',
        instructions: 'Start by picking your water system from the drop-down menu. If necessary, you may narrow your results by adding a company name or the city where the tester is located.',
        showFireLicense: true
    },
    {
        slug: 'csi-inspectors',
        professionalType: ProfessionalType.CsiInspector,
        name: 'CSI Inspectors',
        pageTitle: 'Registered CSI Inspector Search',
        resultsLabel: 'Registered CSI Inspectors',
        instructions: 'Start by picking your water system from the drop-down menu. If necessary, you may narrow your results by adding a company name or the city where the inspector is located.',
        showFireLicense: false
    },
    {
        slug: 'fog-inspectors',
        professionalType: ProfessionalType.FogInspector,
        name: 'FOG Inspectors',
        pageTitle: 'Registered FOG Inspector Search',
        resultsLabel: 'Registered FOG Inspectors',
        instructions: 'Start by picking your water system from the drop-down menu. If necessary, you may narrow your results by adding a company name or the city where the inspector is located.',
        showFireLicense: false
    },
    {
        slug: 'fog-transporters',
        professionalType: ProfessionalType.FogTransporter,
        name: 'FOG Transporters',
        pageTitle: 'Registered FOG Transporter Search',
        resultsLabel: 'Registered FOG Transporters',
        instructions: 'Start by picking your water system from the drop-down menu. If necessary, you may narrow your results by adding a company name or the city where the transporter is located.',
        showFireLicense: false
    }
];

export const DEFAULT_REGISTERED_PROFESSIONAL_PAGE_TITLE = 'Registered Accounts Search';
