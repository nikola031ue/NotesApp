import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { ErrorHandlerService } from 'src/app/shared/error-handler.service';
import { HttpErrorResponse } from '@angular/common/http';
import { UserForRegistrationDto } from 'src/app/models/userForRegistrationDto';
import { RegistrationResponseDto } from 'src/app/models/registrationResponseDto';
import { UserService } from '../user.service';
import { VerificationResponseDto } from 'src/app/models/verificationResponseDto';

function passwordMatcher(c: AbstractControl): { [key: string]: boolean } | null {
  const passwordControl = c.get('password');
  const confirmControl = c.get('confirmPassword');

  if (passwordControl.pristine || confirmControl.pristine) {
    return null;
  }

  if (passwordControl.value === confirmControl.value) {
    return null;
  }
  return { match: true };
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  mouseoverRegister: any;
  showError: boolean;
  errorMessage: string;
  registerForm: FormGroup;

  constructor(private authService:AuthService,
              private router:Router,
              private fb: FormBuilder,
              private errorHandler: ErrorHandlerService,
              private userService: UserService) {}

  ngOnInit() {
    this.registerForm = this.fb.group({
      firstname: ['', [Validators.required]],
      lastname: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      passwordGroup: this.fb.group({
        password: ['', [Validators.required, Validators.minLength(3)]],
        confirmPassword: ['', [Validators.required]],
      }, { validator: passwordMatcher })
    })
  }

  validateControl(controlName: string) {
    return this.registerForm.get(controlName)?.invalid && this.registerForm.get(controlName)?.touched
  }

  hasError(controlName: string, errorName: string) {
    return this.registerForm.get(controlName)?.hasError(errorName)
  }

  verificationCode:VerificationResponseDto;
  registerUser(registerFormValue: any) {
    const formValues = { ...registerFormValue };
    const user: UserForRegistrationDto = {
      firstName: formValues.firstname,
      lastName: formValues.lastname,
      email: formValues.email,
      password: formValues.passwordGroup.password,
    };

    this.authService.registerUser(user)
    .subscribe({
      next: (res: RegistrationResponseDto) => {
        console.log("Successful registration"),
        this.userService.createVerificationCode(res.userId).subscribe({
          next: (resp:VerificationResponseDto) => {
            console.log(resp.verificationCode);
          }
        });
        this.router.navigate(['/login'])
      },
      error: (err: HttpErrorResponse) => {
        console.log(err.error.errors);
        this.errorMessage = this.errorHandler.handleError(err);
        this.showError = true;
      }
    })
  }

  cancel() {
    this.router.navigate(['/login'])
  }
}
