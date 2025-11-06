import { Component, input } from '@angular/core';
import { WindowsMetricDto } from 'src/app/core/utils/models';

@Component({
  selector: 'monitorglass-windows-history',
  imports: [],
  templateUrl: './windows-history.component.html',
  styleUrl: './windows-history.component.css',
})
export class WindowsHistoryComponent {
  public metrics = input<WindowsMetricDto[]>();
  public isLoading = input<boolean>(false, { alias: 'loading' });
}
