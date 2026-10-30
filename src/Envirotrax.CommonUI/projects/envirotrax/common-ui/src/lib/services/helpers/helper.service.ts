import { HttpErrorResponse, HttpStatusCode } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class HelperService {
    public copy<T>(value: T, replacer?: (key: string, value: any) => any): T {
        const json = JSON.stringify(value, replacer);
        return JSON.parse(json);
    }

    public parseValidationErrors(e: any, validationErrors: string[]): boolean {
        if (e instanceof HttpErrorResponse) {
            if (e.status == HttpStatusCode.BadRequest) {
                if (typeof e.error.error == 'string') {
                    validationErrors.push(e.error.error);
                }

                if (typeof e.error == 'string') {
                    validationErrors.push(e.error);
                }

                if (e.error.errors) {
                    for (let key in e.error.errors) {
                        validationErrors.push(e.error.errors[key]);
                    }
                }

                return validationErrors.length > 0;
            }
        }

        return false;
    }

    public isNotFoundError(error: any): boolean {
        if (error instanceof HttpErrorResponse) {
            if (error.status == HttpStatusCode.NotFound) {
                return true;
            }
        }

        return false;
    }

    public loadJavaScript(elementId: string, async: boolean, url: string): Promise<void> {
        const existingScript = document.getElementById(elementId);

        if (existingScript) {
            return Promise.resolve();
        }

        return new Promise<void>((resolve, reject) => {
            const script = document.createElement('script');

            script.id = elementId;
            script.innerHTML = '';
            script.src = url;
            script.async = async;

            script.onload = () => resolve();
            script.onerror = err => reject(err);

            document.body.appendChild(script);
        });
    }

    public isDefined(value: any): boolean {
        return value != undefined && value != null;
    }

    public toNumberOrUndefined(value: unknown): number | undefined {
        if (value === null || value === undefined || value === '') {
            return undefined;
        }

        const parsed = Number(value);

        return Number.isNaN(parsed) ? undefined : parsed;
    }

    public getShortMonthName(month: number | null | undefined): string {
        if (!month || month < 1 || month > 12) {
            return '';
        }

        const date = new Date(2000, month - 1, 1);

        return date.toLocaleString('en-US', { month: 'short' });
    }

    public toTextOrUndefined(value: unknown): string | undefined {
        if (value === null || value === undefined || value === '') {
            return undefined;
        }

        return String(value);
    }

    public buildStreetAddress(streetNumber?: string, streetName?: string, number?: string): string {
        let address = `${streetNumber ?? ''} ${streetName ?? ''}`.trim();

        if (number) {
            address = `${address} #${number}`.trim();
        }

        return address;
    }

    public buildCityStateZip(city?: string, state?: string, zip?: string): string {
        let result = city ?? '';

        if (state) {
            result = result ? `${result}, ${state}` : state;
        }

        if (zip) {
            result = result ? `${result}  ${zip}` : zip;
        }

        return result;
    }
}