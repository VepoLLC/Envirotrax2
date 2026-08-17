import { HttpContextToken, HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import { ModalHelperService, ToastService, ToastType } from "@envirotrax/common-ui";
import { AuthService } from "../auth/auth.service";

// Requests that already treat a particular status as expected control flow (e.g. a 404
// meaning "no record yet") can opt out of the global handling by setting this to true.
export const SKIP_ERROR_INTERCEPTOR = new HttpContextToken<boolean>(() => false);

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {

    constructor(
        private readonly _authService: AuthService,
        private readonly _toastService: ToastService,
        private readonly _modalHelper: ModalHelperService) {
    }

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError((error: HttpErrorResponse) => {
                if (!req.context.get(SKIP_ERROR_INTERCEPTOR)) {
                    this.handleError(error);
                }

                return throwError(() => error);
            })
        );
    }

    private handleError(error: HttpErrorResponse): void {
        switch (error.status) {
            case 401:
                this._authService.signIn();
                break;

            case 403:
                this._toastService.show({
                    text: "You don't have permission to perform this action.",
                    type: ToastType.Error
                });

                break;

            case 404:
                this._toastService.show({
                    text: 'The requested resource could not be found.',
                    type: ToastType.Error
                });

                break;

            case 500: {
                const messages = ['An unexpected error occurred. Please try again.'];
                const traceId = error.error?.traceId;

                if (traceId) {
                    messages.push(`If the problem continues, contact support and include this reference ID: ${traceId}`);
                }

                this._modalHelper.showMessage({
                    title: 'Something Went Wrong',
                    type: 'error',
                    messages
                });

                break;
            }

            case 400: {
                const messages: string[] = [];

                if (typeof error.error === 'string') {
                    messages.push(error.error);
                } else if (error.error?.errors) {
                    messages.push(...Object.values<string[]>(error.error.errors).flat());
                } else if (error.error && typeof error.error === 'object') {
                    const values = Object.values(error.error);
                    if (values.length && values.every(value => Array.isArray(value))) {
                        messages.push(...(values as string[][]).flat());
                    }
                }

                if (messages.length) {
                    this._modalHelper.showMessage({
                        title: 'Validation Error',
                        type: 'error',
                        messages
                    });
                }

                break;
            }
        }
    }
}
