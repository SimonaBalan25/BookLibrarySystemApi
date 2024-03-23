import { HttpClient } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AddAuthorDialogComponent } from 'src/app/dialogs/add-author/add-author.dialog.component';
import { DeleteAuthorDialogComponent } from 'src/app/dialogs/delete-author/delete-author.dialog.component';
import { EditAuthorDialogComponent } from 'src/app/dialogs/edit-author/edit-author.dialog.component';
import { ManageBooksDialogComponent } from 'src/app/dialogs/manage-books/manage-books.dialog.component';
import { Author } from 'src/app/models/author';
import { Book } from 'src/app/models/book';
import { BookBase } from 'src/app/models/book-base';
import { AuthorsService } from 'src/app/services/authors.service';

@Component({
  selector: 'app-authors-list',
  templateUrl: './authors-list.component.html',
  styleUrls: ['./authors-list.component.css']
})
export class AuthorsListComponent {
  displayedColumns = ['name', 'country', 'actions'];

  dataSource: any = [];
  authorsList: Author[] = [];
  exampleDatabase: AuthorsService | null;
  books: Author[] = [];
  index: number=0;
  id: number=0;
  pageIndex: number=0;
  pageSize: number=10;
  totalItems:number=0;
  sortColumn: string='name';
  sortDirection:string='asc';
  @ViewChild('paginator', {static: true}) paginator!: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort = {} as MatSort;

  constructor(private authorsService: AuthorsService, public dialog: MatDialog, private httpClient: HttpClient) {

  }

  ngAfterViewInit(): void {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
  }

  ngOnInit(): void {
    this.loadAuthors();
  }

  loadAuthors() {
    this.exampleDatabase = new AuthorsService(this.httpClient);
    this.authorsService.getBySortCriteria(this.sortColumn, this.sortDirection).subscribe((data) => {
      this.authorsList = data['authors'];
      this.dataSource = new MatTableDataSource<Author>(data['authors']);

      this.dataSource.sort = this.sort;
      this.paginator.length = data['totalItems'];
      this.totalItems = data['totalItems'];
      this.iterator();

    }, error => console.error(error));
  }

  onSortOnHeader(columnName: string) {
    // Check if the clicked column is already the active sorting column
    // Apply sorting
    this.sortColumn = this.dataSource.sort.active;
    this.sortDirection = this.dataSource.sort.direction;
    this.loadAuthors();
  }

  addNew() {
    const dialogRef = this.dialog.open(AddAuthorDialogComponent, {

      data: { version: new Uint8Array(0) }
    });
  }

  startEdit(i: number, id: number, name: string, country: string, bookId: number) {
    this.id = id;
    // index row is used just for debugging proposes and can be removed
    this.index = i;
    console.log(this.index);
    const dialogRef = this.dialog.open(EditAuthorDialogComponent, {
      data: { id: id, name: name, country: country, book: bookId}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 1) {
        // When using an edit things are little different, firstly we find record inside DataService by id
        let foundIndex = this.dataSource?.data.value.findIndex((x:Author) => x.id === this.id);
        // Then you update that record using data from dialogData (values you enetered)
        if (foundIndex != undefined && foundIndex >= 0 && this.exampleDatabase != null) {
          this.exampleDatabase.dataChange.value[foundIndex] = this.authorsService.getDialogData();
        }
        // And lastly refresh table
        this.refreshTable();
      }
    });
  }

  startDelete(i: number, id: number, name: string, country: string, bookId: number) {
    this.index = i;
    this.id = id;
    const dialogRef = this.dialog.open(DeleteAuthorDialogComponent, {
      data: {id: id, name: name, country: country}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 1) {
        let foundIndex = this.exampleDatabase?.dataChange.value.findIndex(x => x.id === this.id);
        // for delete we use splice in order to remove single object from DataService
        if (foundIndex !== undefined && foundIndex > 0) {
            this.exampleDatabase?.dataChange.value.splice(foundIndex, 1);
        }
        this.refreshTable();
      }
    });
  }

  startManage(i: number, id: number, name: string, books: BookBase[]){
    this.index = i;
    this.id = id;
    const dialogRef = this.dialog.open(ManageBooksDialogComponent, {
      data: {id: id, name: name, books: books }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 1) {
        let foundIndex = this.exampleDatabase?.dataChange.value.findIndex(x => x.id === this.id);
        // for delete we use splice in order to remove single object from DataService
        if (foundIndex !== undefined && foundIndex > 0) {
            this.exampleDatabase?.dataChange.value.splice(foundIndex, 1);
        }
        this.refreshTable();
      }
    });
  }


  onPageChanged(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    //this.loadAuthors();
    this.iterator();
  }

  private iterator() {
    const end = (this.pageIndex + 1) * this.pageSize;
    const start = this.pageIndex * this.pageSize;
    const part = this.authorsList.slice(start, end);
    this.dataSource = part;
  }

  private refreshTable() {
    // Refreshing table using paginator
    this.loadAuthors();
    this.paginator._changePageSize(this.paginator.pageSize);
  }
}
