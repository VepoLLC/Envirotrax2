// A returnUrl arriving on a query string is untrusted input - without this check it's an open
// redirect (e.g. ?returnUrl=https://evil.com would send a just-authenticated user off-site).
// Only a root-relative, same-origin path is considered safe.
export function isSafeReturnUrl(url: string | null | undefined): url is string {
    if (!url || !url.startsWith('/') || url.startsWith('//') || url.startsWith('/\\')) {
        return false;
    }

    try {
        return new URL(url, window.location.origin).origin === window.location.origin;
    } catch {
        return false;
    }
}
