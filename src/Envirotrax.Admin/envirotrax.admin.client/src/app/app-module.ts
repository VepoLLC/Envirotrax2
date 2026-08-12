import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { API_BASE_URL, TimeZoneInterceptor } from '@envirotrax/common-ui';
import { environment } from '../environments/environment';
import { SharedComponentsModule } from './shared/components/shared.components.module';
import { AuthInterceptor } from './shared/services/auth/auth.iterceptor';
import { WindowContainerComponent } from './window/window-container.component';
import { WindowComponent } from './window/window.component';
import { HttpErrorInterceptor } from './shared/services/http/http-error.interceptor';

@NgModule({
  declarations: [
    App,
    WindowContainerComponent,
    WindowComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedComponentsModule,
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: API_BASE_URL, useValue: environment.apiUrl },
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
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true
    },
  ],
  bootstrap: [App]
})
export class AppModule { }
