import { inject, Injectable, signal } from '@angular/core';
import {
  AccessTokenResponse,
  LoginRequest,
  UserClient,
  UserDto,
} from 'src/app/web-api-client';
import { ErrorService } from './error.service';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, finalize, of, switchMap, tap } from 'rxjs';

export type TokenWithIsTokeExpired = Omit<
  AccessTokenResponse,
  'init' | 'toJSON' | 'fromJS' | 'expiresIn'
> & { isTokenExpired: boolean };

export type User = Omit<UserDto, 'init' | 'toJSON' | 'fromJS'>;

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private _userClient = inject(UserClient);
  private _errorService = inject(ErrorService);
  private _router = inject(Router);
  private _activatedRoute = inject(ActivatedRoute);

  private _user = signal<User | null>(null);
  public user = this._user.asReadonly();

  private _loginLoader = signal(false);
  public loginLoader = this._loginLoader.asReadonly();

  public getAuthToken = (): TokenWithIsTokeExpired | null => {
    const tokenStore = localStorage.getItem('auth_token');

    if (!tokenStore) return null;

    const {
      accessToken,
      expiresIn,
      refreshToken,
      tokenType,
    }: Omit<AccessTokenResponse, 'init' | 'toJSON' | 'fromJS'> =
      JSON.parse(tokenStore);

    const token: TokenWithIsTokeExpired = {
      accessToken,
      refreshToken,
      tokenType,
      isTokenExpired: Date.now() > expiresIn,
    };

    return token;
  };

  public login(email: string, password: string) {
    this._loginLoader.set(true);
    const loginRequest = new LoginRequest();
    loginRequest.email = email;
    loginRequest.password = password;

    return this._userClient.postApiUserLogin(null, null, loginRequest).pipe(
      tap((tokens) => {
        const authToken: Omit<
          AccessTokenResponse,
          'init' | 'toJSON' | 'fromJS'
        > = {
          accessToken: tokens.accessToken,
          expiresIn: Date.now() + tokens.expiresIn * 1000,
          refreshToken: tokens.refreshToken,
          tokenType: tokens.tokenType,
        };

        const returnUrl: string =
          this._activatedRoute.snapshot.queryParams['ReturnUrl'] ?? '/app';

        localStorage.setItem('auth_token', JSON.stringify(authToken));
        this._router.navigate([returnUrl]);
      }),
      finalize(() => this._loginLoader.set(false)),
      catchError((error) => this._errorService.handleHttpError(error))
    );
  }

  public logout() {
    localStorage.removeItem('auth_token');
    this._user.set(null);
    this._router.navigate(['/identity', 'login']);
  }

  public fetchLoggedInUser = () =>
    this._userClient.me().pipe(
      switchMap((user) => {
        const currentUser: User = {
          displayName: user.displayName,
          emailAddress: user.emailAddress,
          emailVerified: user.emailVerified,
          id: user.id,
          phoneNumber: user.phoneNumber,
          phoneNumberVerified: user.phoneNumberVerified,
          roles: user.roles,
        };

        return of(currentUser);
      }),
      tap((user) => this._user.set(user)),
      catchError((error) => this._errorService.handleUnauthorizeError(error))
    );
}
