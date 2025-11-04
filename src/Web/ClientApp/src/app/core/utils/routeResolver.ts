import { ResolveFn } from '@angular/router';
import { User, UserService } from '../services/user.service';
import { inject } from '@angular/core';
import { catchError, of } from 'rxjs';
import { environment } from 'src/environments/environment';

export const userResolver: ResolveFn<User | null> = () => {
  const userService = inject(UserService);
  if (userService.user()) return of(userService.user());

  return userService.fetchLoggedInUser().pipe(
    catchError((error) => {
      if (!environment.production) {
        console.log(error);
      }
      return of(null);
    })
  );
};
