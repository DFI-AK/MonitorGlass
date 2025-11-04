import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { throwError } from 'rxjs';
import { ProblemDetails, SwaggerException } from 'src/app/web-api-client';

@Injectable({
  providedIn: 'root',
})
export class ErrorService {
  private _messageService = inject(MessageService);
  private _router = inject(Router);

  public handleHttpError(
    error: HttpErrorResponse | SwaggerException | ProblemDetails
  ) {
    const severity =
      error.status === 400 || error.status === 404 ? 'info' : 'error';

    if (error instanceof HttpErrorResponse) {
      if (error.status === 400 || error.status === 404) {
        this._messageService.add({
          summary: 'An application error occured',
          detail: error.message,
          severity,
          life: 4500,
        });
      }

      if (error.status === 401 || error.status === 403) {
        this._messageService.add({
          summary: 'An application error occured',
          detail: error.message,
          severity,
          life: 4500,
        });

        this._router.navigate(['/identity', 'login'], {
          queryParams: { ReturnUrl: window.location.pathname },
        });
      }
    }

    if (error instanceof SwaggerException && severity === 'info') {
      const problemDetails: ProblemDetails = JSON.parse(error.response);
      if (error.status === 400 || error.status === 404) {
        this._messageService.add({
          summary: problemDetails.title,
          detail: problemDetails.detail,
          severity,
          life: 4500,
        });
      }

      if (error.status === 401 || error.status === 403) {
        this._messageService.add({
          summary: problemDetails.title,
          detail: problemDetails.detail,
          severity,
          life: 4500,
        });

        this._router.navigate(['/identity', 'login'], {
          queryParams: { ReturnUrl: window.location.pathname },
        });
      }
    }

    if (error instanceof ProblemDetails && severity === 'info') {
      if (error.status === 400 || error.status === 404) {
        this._messageService.add({
          summary: error.title,
          detail: error.detail,
          severity,
          life: 4500,
        });
      }

      if (error.status === 401 || error.status === 403) {
        this._messageService.add({
          summary: error.title,
          detail: error.detail,
          severity,
          life: 4500,
        });

        this._router.navigate(['/identity', 'login'], {
          queryParams: { ReturnUrl: window.location.pathname },
        });
      }
    }

    return throwError(() => error);
  }

  public handleUnauthorizeError(
    error: HttpErrorResponse | SwaggerException | ProblemDetails
  ) {
    if (error instanceof ProblemDetails && error.status === 401) {
      this._messageService.add({
        summary: error.title,
        severity: 'error',
        detail: error.detail,
        life: 4500,
      });
      this._router.navigate(['/identity', 'login'], {
        queryParams: { ReturnUrl: window.location.pathname },
      });
    } else if (error instanceof ProblemDetails && error.status === 403) {
      this._messageService.add({
        summary: error.title,
        detail: error.detail,
        severity: 'error',
        life: 4500,
      });

      this._router.navigate(['/forbidden'], {
        queryParams: { ReturnUrl: window.location.pathname },
      });
    } else {
      if (error instanceof SwaggerException && error.status === 401) {
        this._messageService.add({
          summary: 'Unauthorize error',
          detail: 'Login session is expired.',
          severity: 'info',
          life: 4500,
        });

        this._router.navigate(['/identity', 'login'], {
          queryParams: { ReturnUrl: window.location.pathname },
        });
      }
    }
    return throwError(() => error);
  }
}
