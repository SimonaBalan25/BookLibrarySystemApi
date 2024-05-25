import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-view-book.dialog',
  templateUrl: './view-book.dialog.component.html',
  styleUrls: ['./view-book.dialog.component.css']
})
export class ViewBookDialogComponent {
  constructor(public dialogRef: MatDialogRef<ViewBookDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, ) {

     }

     onOkClick(): void {
      this.dialogRef.close();
    }
}
