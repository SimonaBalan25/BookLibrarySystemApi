import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageBooksDialogComponent } from './manage-books.dialog.component';

describe('ManageBooksDialogComponent', () => {
  let component: ManageBooksDialogComponent;
  let fixture: ComponentFixture<ManageBooksDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManageBooksDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageBooksDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
