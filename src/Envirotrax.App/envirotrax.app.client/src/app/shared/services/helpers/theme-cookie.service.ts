import { Injectable } from "@angular/core";

const COOKIE_NAME = 'vp-theme';

@Injectable({
    providedIn: 'root'
})
export class ThemeCookieService {

    public get(): 'dark' | 'light' | null {
        for (const cookie of document.cookie.split('; ')) {
            const [name, value] = cookie.split('=');

            if (name === COOKIE_NAME) {
                return value === 'dark' || value === 'light' ? value : null;
            }
        }

        return null;
    }

    public set(theme: 'dark' | 'light'): void {
        const expires = new Date();
        expires.setFullYear(expires.getFullYear() + 1);

        const domain = this.getSharedDomain();
        const domainAttr = domain ? `; domain=${domain}` : '';
        const secureAttr = location.protocol === 'https:' ? '; Secure' : '';

        document.cookie = `${COOKIE_NAME}=${theme}; expires=${expires.toUTCString()}; path=/${domainAttr}; SameSite=Lax${secureAttr}`;
    }

    private getSharedDomain(): string | null {
        const parts = location.hostname.split('.');

        if (parts.length <= 1) {
            return null;
        }

        return '.' + parts.slice(-2).join('.');
    }
}
