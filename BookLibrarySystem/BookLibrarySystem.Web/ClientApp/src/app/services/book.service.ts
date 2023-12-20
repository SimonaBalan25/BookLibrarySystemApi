import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Book } from '../models/book';

@Injectable({
  providedIn: 'root'
})
export class BookService {
   dialogData: any;
   dataChange: BehaviorSubject<Book[]> = new BehaviorSubject<Book[]>([]);

  constructor(private http: HttpClient) {

   }

   getBooks() :Observable<Book[]> {
    return this.http.get(environment.baseApiUrl + '/books/').pipe(
      map((response: any) => {
        console.log(response);
        return response;

      }));
   }

   getBooksBySearchCriteria(pageIndex: number, pageSize: number, sortColumn: string, sortDirection:string, filters: any): Observable<any>{
      let params = new HttpParams()
      .set('pageIndex', pageIndex.toString())
      .set('pageSize', pageSize.toString())
      .set('sortColumn', sortColumn)
      .set('sortDirection', sortDirection);
      //.set('filters', filters);

      // Add filter parameters
      for (const key in filters) {
        if (filters.hasOwnProperty(key)) {
          params = params.set(`filters[${key}]`, filters[key]);
        }
      }

      return this.http.get(environment.baseApiUrl+'/books/getBySearchCriteria', { params });
   }

   getDialogData(){
    return this.dialogData;
   }
}
