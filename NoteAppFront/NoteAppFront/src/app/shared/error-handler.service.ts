import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {


  constructor(private router: Router) { }

  public handleError = (error: HttpErrorResponse) : string => {
    if(error.status === 404) {
      return this.handleNotFound(error);
    }
    else if(error.status === 400) {
      return this.handleBadRequest(error);
    }
    else if(error.status === 401) {
      return this.handleUnauthorized(error);
    }
    return '';
  }

  private handleNotFound(error: HttpErrorResponse) {
    if(this.router.url === '/login') {
      return 'Wrong email or password.';
    }
    else {
      this.router.navigate(['/login']);
      return error.message;
    }
  }

  private handleBadRequest(error: HttpErrorResponse) {
    if(this.router.url === '/login' || this.router.url === '/register') {
      return 'Wrong email format.';
    }
    else {
      this.router.navigate(['/login']);
      return error.message;
    }
  }

  private handleUnauthorized = (error: HttpErrorResponse) => {
    if(this.router.url === '/login') {
      return 'Authentication failed. Wrong Email or Password';
    }
    else {
      this.router.navigate(['/login']);
      return error.message;
    }
  }

}
