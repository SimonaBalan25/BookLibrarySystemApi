import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddAuthorDialogComponent } from './add-author.dialog.component';

describe('AddAuthorDialogComponent', () => {
  let component: AddAuthorDialogComponent;
  let fixture: ComponentFixture<AddAuthorDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddAuthorDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddAuthorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
