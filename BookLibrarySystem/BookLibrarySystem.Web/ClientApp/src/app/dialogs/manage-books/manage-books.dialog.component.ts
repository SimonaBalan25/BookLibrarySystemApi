import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Author } from 'src/app/models/author';
import { AuthorsService } from 'src/app/services/authors.service';
import { MatSelectionList } from '@angular/material/list';
import { Book } from 'src/app/models/book';
import { BookService } from 'src/app/services/book.service';
import { BookBase } from 'src/app/models/book-base';

@Component({
  selector: 'app-manage-books.dialog',
  templateUrl: './manage-books.dialog.component.html',
  styleUrls: ['./manage-books.dialog.component.css']
})
export class ManageBooksDialogComponent implements OnInit {

  @ViewChild('leftList') leftList!: MatSelectionList;
  @ViewChild('rightList') rightList!: MatSelectionList;

  allBooks: BookBase[] = []; // Populate this with all books
  leftBooks: BookBase[] = [];
  assignedBooks: BookBase[] = []; // Initially empty
  response:boolean;

  constructor(public dialogRef: MatDialogRef<ManageBooksDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Author,
    public authorsService: AuthorsService, private bookService: BookService, private snackBar: MatSnackBar) {

  }

  ngOnInit() {
    this.bookService.getBooksForListingAsync().subscribe(result => {
      this.allBooks = result;
      this.assignedBooks = this.allBooks.filter(x => this.data.books.indexOf(x.id) != -1);

      this.leftBooks = this.allBooks.filter(x => !this.assignedBooks.includes(x)).sort((a:BookBase, b:BookBase) => (a.title > b.title) ? 1 : ((b.title > a.title) ? -1 : 0));
    });
  }

  moveToRight() {
    const selectedBooks = this.leftList.selectedOptions.selected.map(option => option.value);
    this.assignedBooks.push(...selectedBooks);
    this.assignedBooks.sort((a:BookBase,b:BookBase) => (a.title > b.title) ? 1 : ((b.title > a.title) ? -1 : 0));

    selectedBooks.forEach(book => {
      const index = this.leftBooks.indexOf(book);
      if (index !== -1) {
        this.leftBooks.splice(index, 1);
      }
    });
    this.leftList.deselectAll();
  }

  moveToLeft() {
    const selectedBooks = this.rightList.selectedOptions.selected.map(option => option.value);
    this.leftBooks.push(...selectedBooks);
    this.leftBooks.sort((a:BookBase,b:BookBase) => (a.title > b.title) ? 1 : ((b.title > a.title) ? -1 : 0));

    selectedBooks.forEach(book => {
      const index = this.assignedBooks.indexOf(book);
      if (index !== -1) {
        this.assignedBooks.splice(index, 1);
      }
    });
    this.rightList.deselectAll();
  }

  onSubmit() {
    // Handle form submission
  }

  confirmAssignBooks(id:number) {
      this.authorsService.assignBooks(this.data.id, this.assignedBooks.map(b=>b.id))
      .subscribe({
        next: data => {
          this.response = data as boolean;
          this.snackBar.open('Successfully assigned', "Okay!", {duration: 3000});
          this.dialogRef.close(1);
        },
        error: err => {
          this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.message, "Okay!", {duration: 8000});
          this.dialogRef.close(0);
        }
      }
      );
  }
}
