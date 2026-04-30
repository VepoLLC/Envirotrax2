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

    public isNotFoundError(error: any): boolean {
        if (error instanceof HttpErrorResponse) {
            if (error.status == 404) {
                return true;
            }
        }

        return false;
    }

    public downloadFileFromUrl(url: string): void {
        const link = document.createElement('a');

        link.href = url;
        link.target = '_blank';
        // this is necessary as link.click() does not work on the latest firefox
        link.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, view: window }));

        window.setTimeout(function () {
            link.remove();
        }, 5000);
    }
}