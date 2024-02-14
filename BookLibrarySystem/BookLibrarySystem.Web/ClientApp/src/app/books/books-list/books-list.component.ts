import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource, MatTableDataSourcePaginator } from '@angular/material/table';
import { Observable } from 'rxjs';
import { AddDialogComponent } from 'src/app/dialogs/add/add.dialog.component';
import { BorrowDialogComponent } from 'src/app/dialogs/borrow/borrow.dialog.component';
import { DeleteDialogComponent } from 'src/app/dialogs/delete/delete.dialog.component';
import { EditDialogComponent } from 'src/app/dialogs/edit/edit.dialog.component';
import { ReturnDialogComponent } from 'src/app/dialogs/return/return.dialog.component';
import { Book } from 'src/app/models/book';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-books-list',
  templateUrl: './books-list.component.html',
  styleUrls: ['./books-list.component.css']
})
export class BooksListComponent implements AfterViewInit {
  displayedColumns = ['title', 'releaseYear', 'genre', 'numberOfPages', 'isbn', 'publisher', 'status', 'actions'];

  dataSource: any = [];
  exampleDatabase: BookService | null;
  books: Book[] = [];
  index: number=0;
  id: number=0;
  pageIndex: number=1;
  pageSize: number=10;
  sortColumn: string='title';
  sortDirection:string='asc';
  filters: {[prop:string]:string} = {
    title: '',
    releaseYear: '',
    genre: '',
    status: '',
  };
  totalItems:number=0;

  @ViewChild('paginator', {static: true}) paginator!: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort = {} as MatSort;

  constructor(private bookService: BookService, public dialog: MatDialog, private httpClient: HttpClient) {

  }

  ngAfterViewInit(): void {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
  }

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks() {
    this.exampleDatabase = new BookService(this.httpClient);
    this.bookService.getBooksBySearchCriteria(this.pageIndex, this.pageSize, this.sortColumn, this.sortDirection, this.filters).subscribe((data) => {
      this.dataSource = new MatTableDataSource<Book>(data['books']);
      //this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.paginator.length = data['totalItems'];
      this.totalItems = data['totalItems'];

    }, error => console.error(error));
  }


  filter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }


  applyServerSideFilter(filters: Record<string, string>) {
    this.filters = filters;
    this.loadBooks();
  }

  addNew() {
    //let newBook = {id: 0, title: '', ISBN: '000-000-000-0', publisher: '', genre: '', loanedQuantity:0, numberOfCopies: 0, numberOfPages:0,releaseYear:0,status:0} as Book;
    const version = new Uint16Array([Date.now()]);
    //newBook.version = version;

    const dialogRef = this.dialog.open(AddDialogComponent, {

      data: { version: new Uint8Array(0) }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 1) {
        // After dialog is closed we're doing frontend updates
        // For add we're just pushing a new row inside DataService
        this.exampleDatabase?.dataChange.value.push(this.bookService.getDialogData());
        this.refreshTable();
      }
    });
  }

  startEdit(i: number, id: number, title: string, releaseYear: number, genre: string, numberOfPages: number, isbn:string, publisher:string, status: string, version: Uint16Array) {
    this.id = id;
    // index row is used just for debugging proposes and can be removed
    this.index = i;
    console.log(this.index);
    const dialogRef = this.dialog.open(EditDialogComponent, {
      data: { id: id, title: title, releaseYear: releaseYear, status: status, numberOfPages: numberOfPages, genre: genre, isbn:isbn, publisher:publisher, version: version }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 1) {
        // When using an edit things are little different, firstly we find record inside DataService by id
        let foundIndex = this.dataSource?.data.value.findIndex((x:Book) => x.id === this.id);
        // Then you update that record using data from dialogData (values you enetered)
        if (foundIndex != undefined && foundIndex >= 0 && this.exampleDatabase != null) {
          this.exampleDatabase.dataChange.value[foundIndex] = this.bookService.getDialogData();
        }
        // And lastly refresh table
        this.refreshTable();
      }
    });
  }

  onPageChanged(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadBooks();
  }

 /* onSortChange(event: any) {
    this.sortColumn = `${event.active}`;
    this.sortDirection = `${event.direction}`;
    this.loadBooks();
  }*/

  onSortOnHeader(columnName: string) {
    // Check if the clicked column is already the active sorting column
    /*if (this.dataSource?.sort?.active === columnName) {
      // Toggle the sorting direction
      this.dataSource.sort.direction =
      this.dataSource.sort.direction === 'asc' ? 'desc' : 'asc';
    } else {
      // Set the new sorting column and direction
      this.dataSource.sort.active = columnName;
      this.dataSource.sort.direction = 'asc';
    }*/

    // Apply sorting
    this.sortColumn = this.dataSource.sort.active;
    this.sortDirection = this.dataSource.sort.direction;
    this.loadBooks();
  }

  startDelete(i: number, id: number, title: string, url: string, publisher: string, genre: string) {
    this.index = i;
    this.id = id;
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      data: {id: id, title: title, url: url, publisher: publisher, genre: genre}
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

  startBorrow(i:number, id: number, title: string) {
    this.index=i;
    this.id=id;
    const dialogRef=this.dialog.open(BorrowDialogComponent, {
      data:{id:id, title: title }
    });
  }

  startReturn(i:number, id:number, title:string){
    this.index=i;
    this.id=id;
    const dialogRef=this.dialog.open(ReturnDialogComponent, {
      data:{id:id, title: title }
    });
  }

  private refreshTable() {
    // Refreshing table using paginator
    this.loadBooks();
    this.paginator._changePageSize(this.paginator.pageSize);
  }
}

export class ExampleDataSource extends DataSource<Book> {

  connect(collectionViewer: CollectionViewer): Observable<readonly Book[]> {
    throw new Error('Method not implemented.');
  }

  disconnect(collectionViewer: CollectionViewer): void {
    throw new Error('Method not implemented.');
  }
}
