import { Component, effect, inject, signal } from '@angular/core';
import { MenubarModule } from 'primeng/menubar';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { UserService } from '../../services/user.service';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'monitorglass-navbar',
  imports: [MenubarModule, MenuModule, ButtonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  private _userService = inject(UserService);
  protected currentUser = this._userService.user;

  protected accountMenu = signal<MenuItem[]>([
    {
      label: 'Menu',
      id: 'menu',
      items: [
        {
          id: 'account_settings',
          label: 'Account settings',
          routerLink: 'account_settings',
          icon: 'pi pi-user',
        },
        {
          id: 'app_settings',
          label: 'Settings',
          routerLink: 'app_settings',
          icon: 'pi pi-cog',
        },
        {
          id: 'configuration',
          label: 'Configuration',
          routerLink: 'configuration',
          icon: 'pi pi-sliders-v',
        },
        {
          separator: true,
        },
        {
          id: 'logout',
          label: 'Logout',
          icon: 'pi pi-sign-out text-red-400!',
          command: () => {
            this._userService.logout();
          },
        },
      ],
    },
  ]);

  constructor() {
    effect(() => {
      this.accountMenu.update((prev) => {
        return prev.map((item) =>
          item.id === 'menu'
            ? {
                ...item,
                label: `${this.currentUser()?.roles[0]}`,
                id: 'menu',
              }
            : item
        );
      });
    });
  }
}
