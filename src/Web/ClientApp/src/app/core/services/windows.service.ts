import { effect, inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, map, of, tap } from 'rxjs';
import {
  AddServerCommand,
  WindowsClient,
  WindowsDto,
} from 'src/app/web-api-client';
import { ErrorService } from './error.service';

export type Windows = Omit<WindowsDto, 'init' | 'fromJS' | 'toJSON'>;

@Injectable({
  providedIn: 'root',
})
export class WindowsService {
  private _windowsClient = inject(WindowsClient);
  private _errorService = inject(ErrorService);

  private _windowsLoader = signal(false);
  public windowsLoader = this._windowsLoader.asReadonly();
  private _windows = signal<Windows | null>(null);
  public windows = this._windows.asReadonly();

  constructor() {
    effect(() => {
      console.log(this._windows());
    });
  }

  public fetchWindows() {
    this._windowsLoader.set(false);
    return this._windowsClient.getWindowsServers().pipe(
      map((servers) => {
        const win: Windows = { ...servers };
        this._windows.set(win);
        return of(win);
      }),
      catchError((err) => this._errorService.handleHttpError(err))
    );
  }

  public destroy() {
    this._windows.set(null);
  }

  public addServer(hostName: string) {
    this._windowsLoader.set(true);
    const command = new AddServerCommand();
    command.serverName = hostName;
    return this._windowsClient.addServer(command).pipe(
      tap((_) => this.fetchWindows()),
      finalize(() => this._windowsLoader.set(false)),
      catchError((err) => this._errorService.handleHttpError(err))
    );
  }

  public deleteWindows(id: string) {
    this._windowsLoader.set(true);
    return this._windowsClient.deleteServer(id).pipe(
      tap((_) => this.fetchWindows()),
      finalize(() => this._windowsLoader.set(false)),
      catchError((err) => this._errorService.handleHttpError(err))
    );
  }
}
