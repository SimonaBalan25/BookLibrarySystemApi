import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { Author } from '../models/author';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthorsService {
  dialogData: any;
  dataChange: BehaviorSubject<Author[]> = new BehaviorSubject<Author[]>([]);

  constructor(private http: HttpClient) { }

  getAuthors(sortColumn: string, sortDirection:string) :Observable<any> {
    return this.http.get(environment.baseApiUrl + 'authors/').pipe(
      map((response: any) => {
        console.log(response);
        return response;

      }));
   }

   getBySortCriteria(sortColumn: string, sortDirection: string) : Observable<any> {
    let params = new HttpParams()
      .set('sortColumn', sortColumn)
      .set('sortDirection', sortDirection);

      return this.http.get(environment.serviceUrl + 'authors/getBySortCriteria', { params });
   }

   addAuthor(newAuthor: Author) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json', // Set the Content-Type to JSON
    });

    return this.http.post(`${environment.serviceUrl}authors/`, newAuthor, {headers});
   }

   updateAuthor(updatedAuthor: Author) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json', // Set the Content-Type to JSON
    });

    return this.http.put(`${environment.serviceUrl}authors/${updatedAuthor.id}`, updatedAuthor, {headers});
   }

   deleteAuthor(id:number) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json', // Set the Content-Type to JSON
    });

    return this.http.delete<boolean>(`${environment.serviceUrl}authors/${id}`, {headers});
   }

   getDialogData(){
    return this.dialogData;
   }

   assignBooks(id:number, bookIds:number[]) {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json', // Set the Content-Type to JSON
    });

    return this.http.put(`${environment.serviceUrl}authors/assign?id=${id}&&booksIds=${bookIds.join(',')}`, {headers});
   }
}
