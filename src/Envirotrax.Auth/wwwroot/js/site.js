// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    var chk = document.getElementById('chkDarkMode');
    if (!chk) { return; }

    chk.checked = localStorage.getItem('vp-theme') === 'dark';

    chk.addEventListener('change', function () {
        if (chk.checked) {
            document.body.classList.add('vp-dark-theme');
            localStorage.setItem('vp-theme', 'dark');
        } else {
            document.body.classList.remove('vp-dark-theme');
            localStorage.setItem('vp-theme', 'light');
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