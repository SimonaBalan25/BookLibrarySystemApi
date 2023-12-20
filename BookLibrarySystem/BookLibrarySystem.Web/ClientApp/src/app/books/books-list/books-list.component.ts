import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Observable } from 'rxjs';
import { EditDialogComponent } from 'src/app/dialogs/edit/edit.dialog.component';
import { Book } from 'src/app/models/book';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-books-list',
  templateUrl: './books-list.component.html',
  styleUrls: ['./books-list.component.css']
})
export class BooksListComponent implements AfterViewInit {
  displayedColumns = ['title', 'releaseYear', 'genre', 'numberOfPages', 'status', 'actions'];

  dataSource: any = [];
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

  @ViewChild('paginator') paginator!: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort = {} as MatSort;

  constructor(private bookService: BookService, public dialog: MatDialog) {

  }

  ngAfterViewInit(): void {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
  }

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(){
    this.bookService.getBooksBySearchCriteria(this.pageIndex, this.pageSize, this.sortColumn, this.sortDirection, this.filters).subscribe((data) => {
      this.dataSource = new MatTableDataSource<Book>(data['books']);//TableVirtualScrollDataSource(data);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
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

  addNew(){

  }

  startEdit(i: number, id: number, title: string, publisher: string, status: string, numberOfPages: number, numberOfCopies: string) {
    this.id = id;
    // index row is used just for debugging proposes and can be removed
    this.index = i;
    console.log(this.index);
    const dialogRef = this.dialog.open(EditDialogComponent, {
      data: { id: id, title: title, publisher: publisher, status: status, numberOfPages: numberOfPages, numberOfCopies: numberOfCopies }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 1) {
        // When using an edit things are little different, firstly we find record inside DataService by id
        const foundIndex = 0;//this.dataSource.dataChange.value.findIndex(x => x.id === this.id);
        // Then you update that record using data from dialogData (values you enetered)
        this.dataSource.dataChange.value[foundIndex] = this.bookService.getDialogData();
        // And lastly refresh table
        this.refreshTable();
      }
    });
  }

  onPageChanged(event: any) {
    this.pageIndex = event.pageIndex+1;
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
    if (this.dataSource.sort.active === columnName) {
      // Toggle the sorting direction
      this.dataSource.sort.direction =
      this.dataSource.sort.direction === 'asc' ? 'desc' : 'asc';
    } else {
      // Set the new sorting column and direction
      this.dataSource.sort.active = columnName;
      this.dataSource.sort.direction = 'asc';
    }

    // Apply sorting
    this.sortColumn = this.dataSource.sort.active;
    this.sortDirection = this.dataSource.sort.direction;
    this.loadBooks();
  }

  startDelete(id: number) {

  }

  startBorrow(id:number){

  }

  private refreshTable() {
    // Refreshing table using paginator
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
