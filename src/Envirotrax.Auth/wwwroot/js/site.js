// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
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