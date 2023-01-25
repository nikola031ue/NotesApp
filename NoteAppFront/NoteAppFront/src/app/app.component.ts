import { Component } from '@angular/core';
import { AuthService } from './user/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'NoteApp';
  public isUserAuthenticated: boolean;
  public isVerified: boolean;

  constructor(public auth:AuthService, private router: Router) {}

  ngOnInit(): void {
    localStorage.clear();
    this.auth.authChanged.subscribe(res => {
      this.isUserAuthenticated = res;
    });

    this.auth.verifChanged.subscribe(res => {
      this.isVerified = res;
    });

    // if(localStorage.getItem("isVerified")) {
    //   this.isVerified = (Boolean(localStorage.getItem("isVerified")) === true)? true : false;
    // }
  }

  public logout() {
    this.auth.logout();
    this.router.navigate(["/"]);
  }
}
