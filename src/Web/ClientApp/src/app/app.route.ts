import { Routes } from '@angular/router';
import { userResolver } from './core/utils/routeResolver';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'app',
    pathMatch: 'full',
  },
  {
    path: 'app',
    resolve: {
      currentUser: userResolver,
    },
    loadComponent: () =>
      import('./core/ui/sidebar-splitter/sidebar-splitter.component').then(
        (c) => c.SidebarSplitterComponent
      ),
    title: 'Monitor Glass',
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./pages/home/home.component').then((c) => c.HomeComponent),
      },
    ],
  },
  {
    path: 'identity',
    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./pages/identity/login/login.component').then(
            (c) => c.LoginComponent
          ),
        title: 'Login',
      },
    ],
  },
  {
    path: 'forbidden',
    loadComponent: () =>
      import('./pages/identity/forbidden/forbidden.component').then(
        (c) => c.ForbiddenComponent
      ),
    title: 'Access denied',
  },
  {
    path: '**',
    redirectTo: 'app',
  },
];
