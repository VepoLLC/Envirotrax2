import { Component, OnInit } from '@angular/core';
import { AuthService } from './shared/services/auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App implements OnInit {
  public isAuthenticated: boolean = false;

  constructor(
    private readonly _authService: AuthService
  ) {

  }

  public ngOnInit(): void {
    this._authService.onLoggedIn().subscribe(async isLoggedIn => {
      this.isAuthenticated = isLoggedIn;

      if (this.isAuthenticated) {

      }
    });
  }
}
