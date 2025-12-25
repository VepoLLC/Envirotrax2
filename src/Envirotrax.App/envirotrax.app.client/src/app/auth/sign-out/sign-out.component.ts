import { Component } from "@angular/core";
import { AuthService } from "../../shared/services/auth/auth.service";

@Component({
    templateUrl: './sign-out.component.html'
})
export class SignOutComponent {
    constructor(private readonly _authService: AuthService) {

    }

    public signIn(): void {
        this._authService.signIn();
    }
}