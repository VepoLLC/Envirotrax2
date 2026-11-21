import { InputOption } from '@envirotrax/common-ui';
import { State } from '../lookup/state';
import { AppUser } from '../users/app-user';

// The values are V1's ContactManagerContacts.Type.
export enum ContactType {
    ExistingClient = 0,
    ProspectiveClient = 1,
    PastClient = 2,
    CompetitorClient = 3,
    Technology = 4
}

export enum NextContactDateStatus {
    None = 0,
    Upcoming = 1,

    // The next contact date is today or has passed.
    Due = 2
}

export const contactTypeLabels: Record<ContactType, string> = {
    [ContactType.ExistingClient]: 'Existing Client',
    [ContactType.ProspectiveClient]: 'Prospective Client',
    [ContactType.PastClient]: 'Past Client',
    [ContactType.CompetitorClient]: 'Competitor Client',
    [ContactType.Technology]: 'Technology'
};

// V1 Vepo Manager colors the person icon by contact type.
export const contactTypeIconClasses: Record<ContactType, string> = {
    [ContactType.ExistingClient]: 'text-primary',
    [ContactType.ProspectiveClient]: 'text-success',
    [ContactType.PastClient]: 'text-secondary',
    [ContactType.CompetitorClient]: 'text-danger',
    [ContactType.Technology]: 'text-warning'
};

export const nextContactDateStatusClasses: Record<NextContactDateStatus, string> = {
    [NextContactDateStatus.None]: '',
    [NextContactDateStatus.Upcoming]: '',
    [NextContactDateStatus.Due]: 'badge bg-danger'
};

export const contactTypeOptions: InputOption[] = [
    ContactType.ExistingClient,
    ContactType.ProspectiveClient,
    ContactType.PastClient,
    ContactType.CompetitorClient,
    ContactType.Technology
].map(type => ({ id: String(type), text: contactTypeLabels[type] }));

export interface Company {
    id?: number;
    contactType?: ContactType;
    name?: string;
    position?: string;
    contactName?: string;
    address?: string;
    city?: string;
    state?: State | null;
    zipCode?: string;
    emailAddress?: string;
    phoneNumber?: string;
    cellNumber?: string;

    hasBackflowTesting?: boolean;
    backflowTestingStartDate?: string | null;
    hasCsiInspection?: boolean;
    csiInspectionStartDate?: string | null;
    hasFogInspection?: boolean;
    fogInspectionStartDate?: string | null;
    hasFogTransportation?: boolean;
    fogTransportationStartDate?: string | null;

    lastContactedBy?: AppUser | null;
    lastContactDate?: string | null;
    nextContactDate?: string | null;
    nextContactDateStatus?: NextContactDateStatus;

    createdTime?: string;
    updatedTime?: string;
}

export interface CompanyProgramBadge {
    title: string;
    color: string;
    fillColor: string;
}

/**
 * A company with its display values precomputed once after load, so the table binds plain fields.
 */
export interface CompanyRow extends Company {
    contactTypeLabel: string;
    contactTypeIconClass: string;
    programs: CompanyProgramBadge[];
    cityState: string;
    nextContactDateClass: string;
}

/**
 * The days the contact calendar shows, as yyyy-MM-dd dates. The end date is the day after the last day shown.
 */
export interface CompanyCalendarRange {
    startDate: string;
    endDate: string;
}
