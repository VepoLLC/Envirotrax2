import { HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable()
export class TimeZoneInterceptor implements HttpInterceptor {

    public intercept(req: HttpRequest<any>, next: HttpHandler) {
        const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

        const cloned = req.clone({
            headers: req.headers.set('EV-TimeZone', timeZone)
        });

        return next.handle(cloned);
    }
}
