import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { SharedComponentsModule } from './shared/components/shared.components.module';
import { TitleStrategy } from '@angular/router';
import { AppTitleStrategy } from './shared/services/helpers/title/app-title-strategy.service';

@NgModule({
  declarations: [
    App
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedComponentsModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: TitleStrategy,
      useClass: AppTitleStrategy
    }
  ],
  bootstrap: [App]
})
export class AppModule { }
