import { HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AuthService } from "./auth.service";
import { from, mergeMap } from "rxjs";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private _authService: AuthService) { }

    public intercept(req: HttpRequest<any>, next: HttpHandler) {
        // Get the auth token from the service.
        return from(this._authService.getAccessToken())
            .pipe(mergeMap(token => {
                if (token) {
                    // Clone the request and replace the original headers with
                    // cloned headers, updated with the authorization.
                    const authReq = req.clone({
                        headers: req.headers.set('Authorization', 'Bearer ' + token)
                    });

                    // send cloned request with header to the next handler.
                    return next.handle(authReq);
                }

                return next.handle(req);
            }));
    }
}