import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AddUserDialogComponent } from 'src/app/dialogs/add-user/add-user.dialog.component';
import { DeactivateUserDialogComponent } from 'src/app/dialogs/deactivate-user/deactivate-user.dialog.component';
import { DeleteUserDialogComponent } from 'src/app/dialogs/delete-user/delete-user.dialog.component';
import { EditUserDialogComponent } from 'src/app/dialogs/edit-user/edit-user.dialog.component';
import { User } from 'src/app/models/user';
import { UserAdditionalInfo } from 'src/app/models/user-additional-info';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.css']
})
export class UsersListComponent implements OnInit {
  displayedColumns = ['name', 'email', 'roles', 'loansNumber', 'reservationsNumber', 'status','actions'];

  dataSource: any = [];
  usersList: User[] = [];
  exampleDatabase: UserService | null;
  //books: BookBase[] = [];
  index: number=0;
  id: number=0;
  pageIndex: number=0;
  pageSize: number=10;
  totalItems:number=0;
  sortColumn: string='name';
  sortDirection:string='asc';
  @ViewChild('paginator', {static: true}) paginator!: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort = {} as MatSort;

  constructor(private usersService: UserService, public dialog: MatDialog, private httpClient: HttpClient,
      ) {


  }

  ngAfterViewInit(): void {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
  }

  ngOnInit(): void {
    this.loadUsersWithAdditionalInfo();
  }

  loadUsersWithAdditionalInfo(){
    this.usersService.loadUsersWithAdditionalInfo().subscribe((data) => {
      this.usersList = data['users'];
      this.dataSource = new MatTableDataSource<UserAdditionalInfo>(data['users']);

      this.dataSource.sort = this.sort;
      this.paginator.length = data['totalItems'];
      this.totalItems = data['totalItems'];
      this.iterator();

    }, error => console.error(error));
  }

  private iterator() {
    const end = (this.pageIndex + 1) * this.pageSize;
    const start = this.pageIndex * this.pageSize;
    const part = this.usersList.slice(start, end);
    this.dataSource = part;
  }

  onSortOnHeader(columnName: string) {
    // Check if the clicked column is already the active sorting column
    // Apply sorting
    this.sortColumn = this.dataSource.sort.active;
    this.sortDirection = this.dataSource.sort.direction;
    this.loadUsersWithAdditionalInfo();
  }

  addNew() {
    const dialogRef = this.dialog.open(AddUserDialogComponent, {


    });
  }

  startEdit(i:number, id:string, name:string, email:string){
    const dialogRef = this.dialog.open(EditUserDialogComponent, {

    });
  }

  startDelete(i:number, id:string){
    const dialogRef = this.dialog.open(DeleteUserDialogComponent,{

    });
  }

  startDeactivate(i:number,id:string){
    const dialogRef = this.dialog.open(DeactivateUserDialogComponent,{

    });
  }


  onPageChanged(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    //this.loadAuthors();
    this.iterator();
  }
}
