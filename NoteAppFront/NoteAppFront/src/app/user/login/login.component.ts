import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthResponseDto } from 'src/app/models/authResponseDto';
import { UserForAuthDto } from 'src/app/models/userForAuthDto';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandlerService } from 'src/app/shared/error-handler.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  mouseoverLogin: any
  loginForm: FormGroup;
  showError: boolean;
  errorMessage: string;
  isVerified: boolean;

  constructor(private authService:AuthService,
              private router:Router,
              private fb: FormBuilder,
              private errorHandler: ErrorHandlerService) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  validateControl(controlName: string) {
    return this.loginForm.get(controlName).invalid && this.loginForm.get(controlName).touched
  }

  hasError(controlName: string, errorName: string) {
    return this.loginForm.get(controlName)!.hasError(errorName)
  }

  loginUser(loginFormValue: any) {
    this.showError = false;
    const login = {... loginFormValue };
    const userForAuth: UserForAuthDto = {
      email: login.email,
      password: login.password
    }
    this.authService.loginUser(userForAuth)
    .subscribe({
      next: (res:AuthResponseDto) => {
        localStorage.setItem("token", res.token);
        localStorage.setItem("id", res.userId);
        this.authService.sendAuthStateChangeNotification(res.isAuthSuccessful);
        // this.isVerified = res.isVerified;
        this.authService.sendVerifState(res.isVerified);
        //localStorage.setItem("isVerified", res.isVerified.toString()); //this.isVerified.toString()
        this.router.navigate(['/notes']);
    },
    error: (err: HttpErrorResponse) => {
      this.errorMessage = this.errorHandler.handleError(err);
      this.showError = true;
    }})
  }
}
