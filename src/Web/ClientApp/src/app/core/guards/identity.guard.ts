import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';

export const identityGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);
  const token = userService.getAuthToken();

  if (!token?.accessToken || token?.isTokenExpired) return true;

  router.navigate(['/app']);
  return false;
};
