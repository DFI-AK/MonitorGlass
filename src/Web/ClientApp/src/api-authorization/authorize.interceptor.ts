import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { UserService } from 'src/app/core/services/user.service';
import { ErrorService } from 'src/app/core/services/error.service';

export const authorizeInterceptor: HttpInterceptorFn = (req, next) => {
  const userService = inject(UserService);
  const errorservice = inject(ErrorService);
  const tokens = userService.getAuthToken();

  if (!tokens?.accessToken || tokens?.isTokenExpired) {
    return next(req).pipe(
      catchError((error) => errorservice.handleUnauthorizeError(error))
    );
  }

  const authReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${tokens?.accessToken}`,
    },
  });

  return next(authReq).pipe(
    catchError((error) => errorservice.handleUnauthorizeError(error))
  );
};
