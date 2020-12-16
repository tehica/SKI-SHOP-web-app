import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-section-header-new',
  templateUrl: './section-header-new.component.html',
  styleUrls: ['./section-header-new.component.scss']
})
export class SectionHeaderNewComponent implements OnInit {

  // whan something is observable then add dollar($) at the end of it
  breadcrumb$: Observable<any[]>;

  constructor(private bcService: BreadcrumbService) { }

  ngOnInit(): void {
    this.breadcrumb$ = this.bcService.breadcrumbs$;
  }

}
