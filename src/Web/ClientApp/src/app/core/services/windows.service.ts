import { inject, Injectable, resource, signal } from '@angular/core';
import {
  catchError,
  delay,
  finalize,
  firstValueFrom,
  map,
  of,
  switchMap,
} from 'rxjs';
import {
  AddServerCommand,
  PaginatedListOfWindowsMetricDto,
  WindowsClient,
  WindowsDto,
} from 'src/app/web-api-client';
import { ErrorService } from './error.service';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { WindowsMetricDto } from '../utils/models';

export type Windows = Omit<WindowsDto, 'init' | 'fromJS' | 'toJSON'>;

export type PaginationForHistorical = Partial<{
  pageNumber: number;
  pageSize: number;
  from: Date;
  to: Date;
}>;

@Injectable({
  providedIn: 'root',
})
export class WindowsService {
  private _windowsClient = inject(WindowsClient);
  private _errorService = inject(ErrorService);

  private _hubConnection: HubConnection | null = null;

  private _windowsLoader = signal(false);
  public windowsLoader = this._windowsLoader.asReadonly();
  private _windows = signal<Windows | null>(null);
  public windows = this._windows.asReadonly();

  private _metric = signal<ReadonlyArray<WindowsMetricDto>>([]);
  public metric = this._metric.asReadonly();

  public historicalPagination = signal<PaginationForHistorical>({
    pageNumber: 1,
    pageSize: 10,
  });
  public historicalMetrics = resource<
    PaginatedListOfWindowsMetricDto,
    PaginationForHistorical
  >({
    params: () => this.historicalPagination(),
    loader: async ({ params }) => {
      const { from, pageNumber, pageSize, to } = params;
      const observable$ = this._windowsClient
        .getHistoricalWindowsMetrics(pageNumber, pageSize, from, to)
        .pipe(
          delay(800),
          catchError((err) => this._errorService.handleUnauthorizeError(err))
        );
      return firstValueFrom(observable$);
    },
  });

  constructor() {
    this.initializeHubconnection();
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

  public async destroy() {
    this._windows.set(null);
    this._metric.set([]);
    this.historicalMetrics.destroy();
    if (this._hubConnection) {
      try {
        await this._hubConnection.stop();
        if (!environment.production) {
          console.log('Hub is disconnected.');
        }
      } finally {
        this._hubConnection = null;
      }
    }
  }

  public addServer(hostName: string) {
    this._windowsLoader.set(true);
    const command = new AddServerCommand();
    command.serverName = hostName;
    return this._windowsClient.addServer(command).pipe(
      switchMap((_) => this.fetchWindows()),
      finalize(() => this._windowsLoader.set(false)),
      catchError((err) => this._errorService.handleHttpError(err))
    );
  }

  public deleteWindows(id: string) {
    this._windowsLoader.set(true);
    return this._windowsClient.deleteServer(id).pipe(
      switchMap((_) => this.fetchWindows()),
      finalize(() => this._windowsLoader.set(false)),
      catchError((err) => this._errorService.handleHttpError(err))
    );
  }

  private buildHubConnection(): HubConnection {
    return new HubConnectionBuilder()
      .withUrl(environment.serverApi + '/api/Windows/windows_metric', {
        transport: HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect([2500, 5000, 10000, 15000, 25000])
      .withKeepAliveInterval(2500)
      .build();
  }

  private async initializeHubconnection() {
    if (
      this._hubConnection &&
      this._hubConnection.state !== HubConnectionState.Disconnected
    ) {
      if (!environment.production) {
        console.log(
          'Hub is already connected with connection id.',
          this._hubConnection?.connectionId
        );
      }
      return;
    }

    this._hubConnection = this.buildHubConnection();

    // ========= Register handlers ==========
    this.registerHandlers();

    this._hubConnection.onreconnecting(() => {
      console.log('Reconnecting to server...');
    });

    this._hubConnection.onreconnected((connectionId) => {
      console.log('Hub is reconncted to connection id.', connectionId);
    });

    this._hubConnection.onclose((error) => {
      if (error) {
        console.log(
          'Connection is closed, will attempt to restart.',
          error.message
        );
      }
      setTimeout(async () => {
        await this.initializeHubconnection();
      }, 3000);
    });

    try {
      await this._hubConnection.start();
      console.log(
        'Hub connection status : ',
        this._hubConnection.state.toString()
      );
    } catch (error) {
      console.log('Connection failed', error);
      setTimeout(async () => {
        await this.initializeHubconnection();
      }, 5000);
    }
  }

  private registerHandlers() {
    this._hubConnection.off('WindowsMetrics');
    this._hubConnection?.on('WindowsMetrics', (metric: WindowsMetricDto) => {
      this._metric.update((prev) => [...prev, metric]);
    });
  }
}
