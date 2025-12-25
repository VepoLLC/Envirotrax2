import { HttpErrorResponse } from "@angular/common/http";
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
            if (e.status == 400) {
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
}