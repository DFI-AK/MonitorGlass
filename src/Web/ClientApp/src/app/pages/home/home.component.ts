import {
  Component,
  computed,
  inject,
  model,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { PopoverModule } from 'primeng/popover';
import { WindowsService } from 'src/app/core/services/windows.service';
import { environment } from 'src/environments/environment';
import { FloatLabel } from 'primeng/floatlabel';
import { InputText } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { ServerCardComponent } from './components/server-card/server-card.component';
import {
  DatetimeRangeComponent,
  RangeSeletor,
} from 'src/app/core/ui/datetime-range/datetime-range.component';
import { WindowsHistoryComponent } from "./components/windows-history/windows-history.component";

@Component({
  selector: 'monitorglass-home',
  templateUrl: './home.component.html',
  imports: [
    ProgressSpinnerModule,
    ButtonDirective,
    ButtonIcon,
    ButtonLabel,
    PopoverModule,
    FloatLabel,
    InputText,
    FormsModule,
    ServerCardComponent,
    DatetimeRangeComponent,
    WindowsHistoryComponent
],
})
export class HomeComponent implements OnDestroy, OnInit {
  private _windowsService = inject(WindowsService);

  protected hostName = model('');
  protected isLoading = this._windowsService.windowsLoader;
  protected windows = this._windowsService.windows;
  protected metric = computed(
    () =>
      this._windowsService.metric()[this._windowsService.metric().length - 1] ??
      null
  );

  protected rangeSelector: RangeSeletor;

  constructor() {}

  ngOnInit(): void {
    this._windowsService.fetchWindows().subscribe({
      error: (error) => {
        if (!environment.production) {
          console.log('An error occured during fetching servers.', error);
        }
      },
    });
  }

  ngOnDestroy(): void {
    this._windowsService.destroy();
  }

  addServer() {
    this._windowsService.addServer(this.hostName()).subscribe({
      error: () => {
        if (!environment.production) {
          console.log('An error occured during adding new server');
        }
      },
    });
  }

  deleteServer(id: string) {
    this._windowsService.deleteWindows(id).subscribe({
      error: () => {
        if (!environment.production) {
          console.log('An error occured during deleting server');
        }
      },
    });
  }

  rangeSelection(value: RangeSeletor) {
    this.rangeSelector = value;
  }
}
