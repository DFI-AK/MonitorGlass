import { DatePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { Windows } from 'src/app/core/services/windows.service';
import { Button } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { Popover } from 'primeng/popover';

@Component({
  selector: 'monitorglass-server-card',
  imports: [CardModule, DatePipe, Button, DividerModule, Popover],
  templateUrl: './server-card.component.html',
  styleUrl: './server-card.component.css',
})
export class ServerCardComponent {
  public server = input<Windows>();
  public deleteEvent = output<string>({ alias: 'onDelete' });

  public onDelete = (id: string) => this.deleteEvent.emit(id);
}
