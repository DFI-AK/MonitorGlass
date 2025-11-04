import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SidebarSplitterComponent } from './sidebar-splitter.component';

describe('SidebarSplitterComponent', () => {
  let component: SidebarSplitterComponent;
  let fixture: ComponentFixture<SidebarSplitterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SidebarSplitterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SidebarSplitterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
