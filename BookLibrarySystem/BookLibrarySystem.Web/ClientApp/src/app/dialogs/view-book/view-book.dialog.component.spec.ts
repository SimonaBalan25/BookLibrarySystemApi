import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewBookDialogComponent } from './view-book.dialog.component';

describe('ViewBookDialogComponent', () => {
  let component: ViewBookDialogComponent;
  let fixture: ComponentFixture<ViewBookDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewBookDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewBookDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
