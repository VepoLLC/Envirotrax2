import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { SharedComponentsModule } from './shared/components/shared.components.module';
import { TitleStrategy } from '@angular/router';
import { AppTitleStrategy } from './shared/services/helpers/title/app-title-strategy.service';
import { AuthInterceptor } from './shared/services/auth/auth.iterceptor';
import { HomeComponent } from './home/home.component';
import { API_BASE_URL, TimeZoneInterceptor } from '@envirotrax/common-ui';
import { environment } from '../environments/environment';

@NgModule({
  declarations: [
    App,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedComponentsModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: API_BASE_URL, useValue: environment.apiUrl },
    {
      provide: TitleStrategy,
      useClass: AppTitleStrategy
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TimeZoneInterceptor,
      multi: true
    },
  ],
  bootstrap: [App]
})
export class AppModule { }
