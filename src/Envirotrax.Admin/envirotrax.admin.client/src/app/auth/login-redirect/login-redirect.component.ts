import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "../../shared/services/auth/auth.service";

@Component({
    templateUrl: './login-redirect.component.html',
    standalone: false
})
export class LoginRedirectComponent implements OnInit {
    public isLoading: boolean = false;

    constructor(
        private readonly _authService: AuthService,
        private readonly _router: Router) {

    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            await this._authService.signInCallback();

            this._authService.setLoggedIn(true);

            this._router.navigateByUrl('/', {
                replaceUrl: true
            });
        } finally {
            this.isLoading = false;
        }
    }
}