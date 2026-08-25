export enum ComplianceOverdueSeverity {
    None = 0,
    DueToday = 1,
    Low = 2,
    Moderate = 3,
    High = 4
}

/**
 * Badge classes for the "Days Overdue" cell on every compliance-management grid.
 *
 * The day thresholds that produce each tier are computed on the back end and differ per domain (the
 * backflow grid uses a tighter scale than the CSI/FOG grids), so only the tiers reach the client. `None`
 * maps to no class because those rows render no badge at all.
 */
// Declared as a numeric index signature (matching reviewDateStatusClasses on the shared property-log cell)
// so templates can index it with an untyped `rowData` field, while `satisfies` still forces every tier to be
// given a class if the enum grows.
export const complianceOverdueSeverityClasses: { [key: number]: string } = {
    [ComplianceOverdueSeverity.None]: '',
    [ComplianceOverdueSeverity.DueToday]: 'bg-secondary',
    [ComplianceOverdueSeverity.Low]: 'bg-warning-subtle text-dark border',
    [ComplianceOverdueSeverity.Moderate]: 'bg-warning text-dark',
    [ComplianceOverdueSeverity.High]: 'bg-danger'
} satisfies Record<ComplianceOverdueSeverity, string>;
