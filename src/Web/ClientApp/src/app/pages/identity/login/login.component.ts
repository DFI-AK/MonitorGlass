import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { UserService } from 'src/app/core/services/user.service';
import { environment } from 'src/environments/environment';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'monitorglass-login',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    FloatLabelModule,
    InputTextModule,
    ButtonModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private _fb = inject(FormBuilder);
  private _userService = inject(UserService);
  protected loader = this._userService.loginLoader;

  protected loginForm = this._fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  public onSubmit() {
    const email = this.email?.value;
    const password = this.password?.value;
    this._userService.login(email, password).subscribe({
      error: (error) => {
        if (!environment.production) {
          console.log(error);
        }
      },
    });
  }
}
