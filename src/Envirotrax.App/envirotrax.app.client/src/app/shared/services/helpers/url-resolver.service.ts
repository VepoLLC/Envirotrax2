import { Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment";

@Injectable({
    providedIn: 'root'
})
export class UrlResolverService {
    public resolveUrl(relativePath: string): string {
        if (relativePath[0] == '/') {
            relativePath = relativePath.substring(1);
        }

        return environment.apiUrl + '/' + relativePath;
    }
}