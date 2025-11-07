import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';

export const appGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);
  const token = userService.getAuthToken();

  if (!token?.accessToken || token?.isTokenExpired) {
    router.navigate(['/identity', 'login'], {
      queryParams: { ReturnUrl: state.url },
    });
    return false;
  }

  return true;
};
