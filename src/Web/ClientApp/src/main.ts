import {
  enableProdMode,
  APP_ID,
  importProvidersFrom,
  Provider,
  EnvironmentProviders,
} from '@angular/core';

import { environment } from './environments/environment.prod';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app.component';
import { routes } from './app/app.route';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { MessageService } from 'primeng/api';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

const providers: (Provider | EnvironmentProviders)[] = [
  { provide: 'API_BASE_URL', useFactory: getBaseUrl, deps: [] },
];

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    ...providers,
    importProvidersFrom(BrowserModule, FormsModule),
    { provide: APP_ID, useValue: 'ng-cli-universal' },
    provideHttpClient(withInterceptors([authorizeInterceptor])),
    provideRouter(routes),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: Aura,
      },
    }),
    MessageService,
  ],
}).catch((err) => console.log(err));
