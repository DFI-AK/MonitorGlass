import { DatePipe, NgClass } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { Windows } from 'src/app/core/services/windows.service';
import { Button } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { Popover } from 'primeng/popover';
import { WindowsMetricDto } from 'src/app/core/utils/models';
import { Divider } from 'primeng/divider';
import { ByteConvertorPipe } from 'src/app/core/pipes/byte-convertor.pipe';

@Component({
  selector: 'monitorglass-server-card',
  imports: [
    CardModule,
    DatePipe,
    Button,
    DividerModule,
    Popover,
    Divider,
    ByteConvertorPipe,
    NgClass,
  ],
  templateUrl: './server-card.component.html',
  styleUrl: './server-card.component.css',
})
export class ServerCardComponent {
  public server = input<Windows>();
  public metric = input<WindowsMetricDto | null>(null);

  protected ramColorIndicator = computed(() => {
    const percentage =
      (this.metric().memory.usedMemoryMB / this.metric().memory.totalMemoryMB) *
      100;

    return percentage < 50 || percentage <= 60
      ? 'text-green-400'
      : percentage <= 83
      ? 'text-amber-500'
      : 'text-red-400';
  });

  public deleteEvent = output<string>({ alias: 'onDelete' });
  public onDelete = (id: string) => this.deleteEvent.emit(id);
}
