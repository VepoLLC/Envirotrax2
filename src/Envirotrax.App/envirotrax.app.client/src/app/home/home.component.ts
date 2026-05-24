import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "../shared/services/auth/auth.service";
import { ROLE_DEFINITIONS } from "../shared/models/role-definitions";

@Component({
    template: '',
    standalone: false
})
export class HomeComponent implements OnInit {
    constructor(
        private readonly _authService: AuthService,
        private readonly _router: Router
    ) { }

    public async ngOnInit(): Promise<void> {
        const isProfessional = await this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONAL);
        if (isProfessional) {
            await this._router.navigate(['/professionals/dashboard'], { replaceUrl: true });
        } else {
            await this._router.navigate(['/dashboard'], { replaceUrl: true });
        }
    }
}
