import { AfterContentInit, AfterViewChecked, AfterViewInit, Component, DoCheck, OnDestroy, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { Observable, Subscription, map, switchMap, take, of, catchError } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { MatIconRegistry } from '@angular/material/icon';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})

export class NavMenuComponent implements OnInit, AfterViewInit, OnDestroy {
  isExpanded = false;
  userRoles:string[] = [];//this.authorizeService.getUserRoles();//
  userToken:string|null='';
  subscriptions$: Subscription[]=[];

  constructor(private userService:UserService,private authorizeService: AuthorizeService)
  {

  }

  ngOnInit() :void {


    this.subscriptions$.push(
    this.authorizeService.isAuthenticated().pipe(switchMap(loggedIn => {
      if(loggedIn) {
        return this.authorizeService.getUserRoles().pipe(
          catchError(error => {
            console.error('Error in getUserRoles:', error);
            return of([]); // Return an empty array or handle the error as needed
          })
        );
      }
      return of([]);
    })).subscribe((roles)=>{
      console.log('Roles received:', roles);
      this.userRoles = roles;
    }
    )
    );
  }

  ngAfterViewInit(): void {
    //this.isAuthenticated = true;

  }

  ngOnDestroy(): void {
    this.subscriptions$.forEach(s=>s?.unsubscribe());
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
