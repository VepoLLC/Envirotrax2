import { SortOperator } from '@envirotrax/common-ui';
import {
    SubmissionValidationFlag,
    SubmissionValidationReason,
    SubmissionValidationReasonFlag
} from '../../shared/models/validations/submission-validation';

// V1 opens on Record Date, newest first.
export const DEFAULT_SORT: { [key: string]: SortOperator } = { createdTime: 'Desc' };

export const NEEDS_VALIDATION_FLAG: SubmissionValidationFlag = {
    label: 'Needs Validation',
    badgeClass: 'vp-validation-badge-red'
};

export const ON_HOLD_FLAG: SubmissionValidationFlag = {
    label: 'Validation On Hold',
    badgeText: 'H',
    badgeClass: 'vp-validation-badge-red'
};

export const REASON_FLAGS: SubmissionValidationReasonFlag[] = [
    {
        reason: SubmissionValidationReason.NewSite,
        label: 'New Site',
        badgeIconClass: 'fa-solid fa-star',
        badgeClass: 'vp-validation-badge-orange'
    },
    {
        reason: SubmissionValidationReason.SiteInformationChanged,
        label: 'Site Information Changed',
        badgeText: 'i',
        badgeClass: 'vp-validation-badge-orange'
    },
    {
        reason: SubmissionValidationReason.CreatedFromCsi,
        label: 'Created from CSI',
        badgeText: 'C',
        badgeClass: 'vp-validation-badge-blue'
    },
    {
        reason: SubmissionValidationReason.DeviceInformationChanged,
        label: 'Device Information Changed',
        badgeText: 'i',
        badgeClass: 'vp-validation-badge-green'
    },
    {
        reason: SubmissionValidationReason.DeviceMoved,
        label: 'Device Moved',
        badgeText: 'M',
        badgeClass: 'vp-validation-badge-green'
    },
    {
        reason: SubmissionValidationReason.HazardTypeChanged,
        label: 'Hazard Type Changed',
        badgeText: 'H',
        badgeClass: 'vp-validation-badge-yellow'
    },
    {
        reason: SubmissionValidationReason.ReceiverInformationChanged,
        label: 'FOG Receiver Information Changed',
        badgeText: 'I',
        badgeClass: 'vp-validation-badge-magenta'
    },
    {
        reason: SubmissionValidationReason.UnknownSerialNumber,
        label: 'Unknown Serial Number',
        badgeText: 'U',
        badgeClass: 'vp-validation-badge-outline-green'
    }
];

export const SITE_REASONS: SubmissionValidationReason[] = [
    SubmissionValidationReason.NewSite,
    SubmissionValidationReason.SiteInformationChanged
];

export const BACKFLOW_REASONS: SubmissionValidationReason[] = [
    ...SITE_REASONS,
    SubmissionValidationReason.CreatedFromCsi,
    SubmissionValidationReason.DeviceInformationChanged,
    SubmissionValidationReason.DeviceMoved,
    SubmissionValidationReason.HazardTypeChanged,
    SubmissionValidationReason.UnknownSerialNumber
];

export const TRIP_TICKET_REASONS: SubmissionValidationReason[] = [
    ...SITE_REASONS,
    SubmissionValidationReason.ReceiverInformationChanged
];

export function getReasonFlags(reasons: SubmissionValidationReason[]): SubmissionValidationReasonFlag[] {
    return REASON_FLAGS.filter(flag => reasons.includes(flag.reason));
}
