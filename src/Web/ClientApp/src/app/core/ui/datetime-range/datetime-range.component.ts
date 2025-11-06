import { Component, effect, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePicker } from 'primeng/datepicker';
import { FloatLabel } from 'primeng/floatlabel';
import { SelectModule } from 'primeng/select';

export type RangeSeletor = Partial<{
  label: string;
  enableRange: boolean;
  value: Date | null;
}>;

@Component({
  selector: 'monitorglass-datetime-range',
  imports: [FormsModule, DatePicker, FloatLabel, SelectModule],
  templateUrl: './datetime-range.component.html',
  styleUrl: './datetime-range.component.css',
})
export class DatetimeRangeComponent {
  protected dateRange = model<Date[]>([]);
  protected selectedOption = model<RangeSeletor>();

  public selectionEvent = output<RangeSeletor>({ alias: 'rangeEvent' });
  protected selectionHistory = (value: RangeSeletor) =>
    this.selectionEvent.emit(value);

  protected options: RangeSeletor[] = [
    {
      label: 'Live',
      enableRange: false,
      value: null,
    },
    {
      label: '5 Minutes',
      value: new Date(Date.now() - 5 * 60 * 1000),
    },
    {
      label: '15 Minutes',
      value: new Date(Date.now() - 15 * 60 * 1000),
    },
    {
      label: '30 Minutes',
      value: new Date(Date.now() - 30 * 60 * 1000),
    },
    {
      label: '1 Hour',
      value: new Date(Date.now() - 60 * 60 * 1000),
    },
    {
      label: '3 Hours',
      value: new Date(Date.now() - 3 * 60 * 60 * 1000),
    },
    {
      label: '6 hours',
      value: new Date(Date.now() - 6 * 60 * 60 * 1000),
    },
    {
      label: 'Custom',
      enableRange: true,
      value: null,
    },
  ];

  constructor() {
    this.selectedOption.set(this.options[0]);

    effect(() => {
      if (this.selectedOption()) {
        this.selectionHistory(this.selectedOption());
      }
    });
  }
}
