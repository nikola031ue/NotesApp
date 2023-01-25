import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorHandlerService } from 'src/app/shared/error-handler.service';
import { AuthService } from '../auth.service';
import { UserService } from '../user.service';
import { HttpErrorResponse } from '@angular/common/http';

function passwordMatcher(c: AbstractControl): { [key: string]: boolean } | null {
  const passwordControl = c.get('newPassword');
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
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  mouseoverChange: any;
  showError: boolean;
  errorMessage: string;
  passwordForm: FormGroup;

  constructor(private authService:AuthService,
    private router:Router,
    private fb: FormBuilder,
    private errorHandler: ErrorHandlerService,
    private userService: UserService) {}

  ngOnInit(): void {
    this.passwordForm = this.fb.group({
      oldPassword: ['', [Validators.required]],
      passwordGroup: this.fb.group({
        newPassword: ['', [Validators.required, Validators.minLength(3)]],
        confirmPassword: ['', [Validators.required]],
      }, { validator: passwordMatcher })
    })
  }

  changePassword(passwordFormValue: any) {
    const id = localStorage.getItem("id");
    let body = [
      {
        "op": "replace",
        "path": "/password",
        "value": passwordFormValue.passwordGroup.newPassword
      }
    ];
    this.userService.changePassword(id, body).subscribe({
      next: () => {
        console.log("Successful changed password.");
        this.authService.logout();
        this.router.navigate(['/login']);
      },
      error: (err: HttpErrorResponse) => {
        console.log(this.errorHandler.handleError(err))
      }
    })
  }

  validateControl(controlName: string) {
    return this.passwordForm.get(controlName)?.invalid && this.passwordForm.get(controlName)?.touched
  }

  hasError(controlName: string, errorName: string) {
    return this.passwordForm.get(controlName)?.hasError(errorName)
  }

  cancel() {
    this.router.navigate(['/login'])
  }
}
