import { Component, input, output } from '@angular/core';
import { PaginationForHistorical } from 'src/app/core/services/windows.service';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { WindowsMetricDto } from 'src/app/web-api-client';
import { DatePipe, NgStyle } from '@angular/common';
import { ButtonDirective } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'monitorglass-windows-history',
  imports: [TableModule, DatePipe, ButtonDirective, SkeletonModule, NgStyle],
  templateUrl: './windows-history.component.html',
  styleUrl: './windows-history.component.css',
})
export class WindowsHistoryComponent {
  public metrics = input<WindowsMetricDto[]>();
  public metricTotalRecord = input<number>();
  public isLoading = input<boolean>(false, { alias: 'loading' });
  public pagination = input<PaginationForHistorical>();

  private cpu_n_memory_headers: Record<
    keyof Omit<
      WindowsMetricDto & { actionButton: any },
      'diskDetails' | 'networkDetails' | 'init' | 'toJSON'
    >,
    string
  > = {
    serverId: 'Server id',
    cpu: 'CPU usage',
    memory: 'Memory usage',
    actionButton: 'Disk/Network',
    created: 'Captured on',
  };

  protected cpu_n_memory_columns = Object.entries(
    this.cpu_n_memory_headers
  ).map((item) => ({ field: item[1], key: item[0] }));

  paginationEvent = output<PaginationForHistorical>();

  changePagination(event: TableLazyLoadEvent) {
    const first = event.first ?? 0;
    const rows = event.rows ?? 10;
    const pageNumber = Math.floor(first / rows) + 1;

    // Only emit if we're truly scrolling to a new page
    if (pageNumber !== this.pagination()?.pageNumber) {
      this.paginationEvent.emit({ pageNumber, pageSize: rows });
    }
  }
}
