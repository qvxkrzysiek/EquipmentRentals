import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { CardModule } from '@progress/kendo-angular-layout';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  standalone: true,
  imports: [
    CommonModule,
    ButtonsModule,
    InputsModule,
    CardModule,
    ReactiveFormsModule
  ]
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;
      this.authService.login(email, password).subscribe({
        next: (response) => {
          if (response?.token) {
            this.authService.saveToken(response.token);

            if (response.role === 'Admin') {
              this.router.navigate(['admin']);
            } else {
              this.router.navigate(['home']);
            }
          } else {
            console.error('Brak tokena w odpowiedzi');
          }
        },
        error: (err) => {
          console.error('Błąd logowania:', err);
        }
      });
    }
  }
}