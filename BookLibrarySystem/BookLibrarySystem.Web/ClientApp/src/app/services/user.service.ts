import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseApiUrl : string = environment.baseApiUrl;

  constructor(private http: HttpClient)
  {
   }

  getUserRoles(token:string|null): Observable<string[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<string[]>(this.baseApiUrl + 'Users/user-roles', {headers});
  }
}
