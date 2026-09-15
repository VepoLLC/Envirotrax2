// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function getThemeCookie() {
    var match = document.cookie.match(/(?:^|;\s*)vp-theme=([^;]*)/);
    return match ? match[1] : null;
}

function setThemeCookie(theme) {
    var expires = new Date();
    expires.setFullYear(expires.getFullYear() + 1);
    var parts = location.hostname.split('.');
    var domainAttr = parts.length > 1 ? '; domain=.' + parts.slice(-2).join('.') : '';
    var secureAttr = location.protocol === 'https:' ? '; Secure' : '';
    document.cookie = 'vp-theme=' + theme + '; expires=' + expires.toUTCString() + '; path=/' + domainAttr + '; SameSite=Lax' + secureAttr;
}

document.addEventListener('DOMContentLoaded', function () {
    var chk = document.getElementById('chkDarkMode');
    if (!chk) { return; }

    chk.checked = getThemeCookie() === 'dark';

    chk.addEventListener('change', function () {
        if (chk.checked) {
            document.body.classList.add('vp-dark-theme');
            setThemeCookie('dark');
        } else {
            document.body.classList.remove('vp-dark-theme');
            setThemeCookie('light');
        }
    });
});

function showHidePassword(event, passwordId) {
    var passwordElement = document.getElementById(passwordId);
    var eyeIcon = event.querySelector('i');

    if (passwordElement.type == 'password') {
        passwordElement.type = 'text';
        eyeIcon.className = 'fa fa-eye-slash'
    } else {
        passwordElement.type = 'password';
        eyeIcon.className = "fa fa-eye"
    }
}

var loadingSpinnerSuppressedUntil = 0;

function showLoadingSpinner() {
    var overlay = document.getElementById('divLoadingSpinner');

    if (!overlay) {
        return;
    }

    overlay.classList.add('vp-loading-overlay-visible');
}

function hideLoadingSpinner() {
    var overlay = document.getElementById('divLoadingSpinner');

    if (!overlay) {
        return;
    }

    overlay.classList.remove('vp-loading-overlay-visible');
}

// Downloads, mailto/tel links and links opened in another tab can fire beforeunload without ever
// replacing the current page, which would leave the overlay on screen forever. Clicking one of them
// suppresses the next spinner instead.
function suppressLoadingSpinner() {
    loadingSpinnerSuppressedUntil = Date.now() + 2000;
}

function keepsCurrentPage(link) {
    if (link.hasAttribute('download')) {
        return true;
    }

    if (link.target && link.target !== '_self') {
        return true;
    }

    if (link.protocol !== 'http:' && link.protocol !== 'https:') {
        return true;
    }

    return !!link.hash && link.href.split('#')[0] === location.href.split('#')[0];
}

document.addEventListener('click', function (event) {
    if (!event.target || !event.target.closest) {
        return;
    }

    var link = event.target.closest('a[href]');

    if (!link) {
        return;
    }

    if (keepsCurrentPage(link)) {
        suppressLoadingSpinner();

        return;
    }

    if (event.defaultPrevented || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) {
        return;
    }

    showLoadingSpinner();
});

document.addEventListener('submit', function (event) {
    if (event.target.hasAttribute('data-no-spinner')) {
        suppressLoadingSpinner();

        return;
    }

    if (event.defaultPrevented) {
        return;
    }

    showLoadingSpinner();
});

// Navigations started by the browser or by script never reach the handlers above. The spinner shown
// here can stay unpainted when the browser is already leaving the page, so it is only a fallback.
window.addEventListener('beforeunload', function () {
    if (Date.now() < loadingSpinnerSuppressedUntil) {
        return;
    }

    showLoadingSpinner();
});

window.addEventListener('pageshow', function () {
    hideLoadingSpinner();
});
