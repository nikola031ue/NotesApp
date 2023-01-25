import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorHandlerService } from 'src/app/shared/error-handler.service';
import { AuthService } from 'src/app/user/auth.service';
import { UserService } from '../../user.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-verification',
  templateUrl: './verification.component.html',
  styleUrls: ['./verification.component.css']
})
export class VerificationComponent {
  verificationForm: FormGroup;
  mouseoverLogin: any;
  showError: boolean = false;
  errorMessage: string;

  constructor(private authService:AuthService,
              private router:Router,
              private fb: FormBuilder,
              private errorHandler: ErrorHandlerService,
              private userService: UserService) {}

  ngOnInit():void {
    this.verificationForm = this.fb.group({
      code: ['', [Validators.required]]
    });
  }

  verifyUser(verificationForm: any) {
    const id = localStorage.getItem("id");
    let body =
      [
        {
          "op": "replace",
          "path": "/verified",
          "value": "true"
        }
      ];

    this.userService.verify(id, verificationForm.code, body).subscribe({
      next: () => {
        console.log("Successful verification.");
        localStorage.setItem("isVerified", "true");
        //this.authService.logout();
        this.router.navigate(['/notes']);
      },
      error: (err: HttpErrorResponse) => {
        if(err.status === 204) {
          this.errorMessage = "User already verified.";
          console.log("User already verified.");
          this.router.navigate(['/notes']);
        }
        //console.log(this.errorHandler.handleError(err))
      }
    })
  }

  validateControl(controlName: string) {
    return this.verificationForm.get(controlName).invalid && this.verificationForm.get(controlName).touched
  }

  hasError(controlName: string, errorName: string) {
    return this.verificationForm.get(controlName)!.hasError(errorName)
  }
}
