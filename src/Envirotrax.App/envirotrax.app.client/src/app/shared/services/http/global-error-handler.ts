import { ErrorHandler, Injectable } from "@angular/core";
import { HttpErrorResponse, HttpStatusCode } from "@angular/common/http";
import { ModalHelperService, ToastService, ToastType } from "@envirotrax/common-ui";
import { AuthService } from "../auth/auth.service";

// Angular only calls handleError for errors nobody else caught - an Observable/Promise
// error handled locally (a subscribe error callback, a try/catch) never reaches this class.
// That is intentional: it lets call sites decide, case by case, whether a failure is
// expected and should be swallowed or handled locally, without needing to opt out of a
// global interceptor that would otherwise always run.
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

    constructor(
        private readonly _authService: AuthService,
        private readonly _toastService: ToastService,
        private readonly _modalHelper: ModalHelperService) {
    }

    public handleError(error: unknown): void {
        const httpError = this.asHttpErrorResponse(error);

        if (!httpError) {
            console.error(error);
            return;
        }

        switch (httpError.status) {
            case HttpStatusCode.Unauthorized:
                this._authService.signIn(undefined, undefined, window.location.pathname + window.location.search + window.location.hash);
                break;

            case HttpStatusCode.Forbidden:
                this._toastService.show({
                    text: "You don't have permission to perform this action.",
                    type: ToastType.Error
                });

                break;

            case HttpStatusCode.NotFound:
                this._toastService.show({
                    text: 'The requested resource could not be found.',
                    type: ToastType.Error
                });

                break;

            case HttpStatusCode.InternalServerError: {
                const messages = ['An unexpected error occurred. Please try again.'];
                const traceId = httpError.error?.traceId;

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

            case HttpStatusCode.BadRequest: {
                const messages: string[] = [];

                if (typeof httpError.error === 'string') {
                    messages.push(httpError.error);
                } else if (httpError.error?.errors) {
                    messages.push(...Object.values<string[]>(httpError.error.errors).flat());
                } else if (httpError.error && typeof httpError.error === 'object') {
                    const values = Object.values(httpError.error);
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

    private asHttpErrorResponse(error: unknown): HttpErrorResponse | undefined {
        const candidate = (error as { rejection?: unknown })?.rejection ?? error;

        return candidate instanceof HttpErrorResponse ? candidate : undefined;
    }
}
