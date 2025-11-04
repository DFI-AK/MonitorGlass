import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SplitterModule } from 'primeng/splitter';
import { NavbarComponent } from "../navbar/navbar.component";

@Component({
  selector: 'monitorglass-sidebar-splitter',
  imports: [SplitterModule, RouterOutlet, NavbarComponent],
  templateUrl: './sidebar-splitter.component.html',
  styleUrl: './sidebar-splitter.component.css',
})
export class SidebarSplitterComponent {}
