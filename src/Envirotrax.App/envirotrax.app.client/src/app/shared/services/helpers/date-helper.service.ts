import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class DateHelperService {

    /**
     * Parses a server timestamp as UTC.
     *
     * EF Core loses DateTimeKind on the round trip through SQL Server, so timestamps come back serialized
     * without a "Z" marker (e.g. "2026-08-21T09:54:25"). The browser's Date parser treats a date-time with
     * no offset as LOCAL time, which shifts the displayed value by the viewer's UTC offset. Values that
     * already carry a marker (a "Z", or a "+04:00"/"-0500" offset) are passed through untouched, so a
     * client-generated `new Date().toISOString()` is never double-suffixed.
     */
    public toUtcDate(isoString?: string): Date | undefined {
        if (!isoString) {
            return undefined;
        }

        const hasTimezone = /[zZ]|[+-]\d{2}:?\d{2}$/.test(isoString);

        return new Date(hasTimezone ? isoString : `${isoString}Z`);
    }
}
