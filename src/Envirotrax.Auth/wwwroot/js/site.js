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
