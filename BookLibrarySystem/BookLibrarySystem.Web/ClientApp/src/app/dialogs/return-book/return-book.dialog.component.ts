import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Book } from 'src/app/models/book';
import { User } from 'src/app/models/user';
import { BookService } from 'src/app/services/book.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-return.dialog',
  templateUrl: './return-book.dialog.component.html',
  styleUrls: ['./return-book.dialog.component.css']
})

export class ReturnBookDialogComponent implements OnInit {
  selectedUsers: User[];
  selectedUser:string;

  constructor(public dialogRef: MatDialogRef<ReturnBookDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Book,
    public bookService: BookService, public userService:UserService, private snackBar: MatSnackBar) { }

  ngOnInit() {
    this.selectedUser = 'no-option';
    this.userService.getUsersAsync().subscribe(data => {
        this.selectedUsers = data;
    });
  }

  submit() {
    // empty stuff
  }

  confirmReturn(selectedUser:string) {
    this.bookService.returnBook(this.data.id, this.selectedUser).subscribe({
      next: data => {
        this.snackBar.open("Successfully returned", "Okay!", {duration: 3000});
        this.dialogRef.close(1);
    },
    error:err => {
      this.snackBar.open('Error occurred. Details: ' + err.name + ' ' + err.error, "Okay!", {duration: 8000});
      this.dialogRef.close(0);
    }
   });
  }

}
