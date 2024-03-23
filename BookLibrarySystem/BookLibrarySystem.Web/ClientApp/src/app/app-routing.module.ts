import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { BooksListComponent } from './books/books-list/books-list.component';
import { UsersListComponent } from './users/users-list/users-list.component';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorsListComponent } from './authors/authors-list/authors-list.component';

const routes: Routes = [
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthorizeGuard] },
      { path: 'books-list', component: BooksListComponent, canActivate: [AuthorizeGuard] },
      { path: 'authors-list', component: AuthorsListComponent, canActivate: [AuthorizeGuard] },
      { path: 'users-list', component: UsersListComponent, canActivate: [AuthorizeGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
