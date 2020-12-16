import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  busyRequestCount = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  /*
    for different type of loading spinner in Angular app change name of type: '' in busy method

    on this link we can see all available type of spinners:
    https://napster2210.github.io/ngx-spinner/
  */
  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'line-spin-clockwise-fade',
      bdColor: 'rgba(255,255,255,0.7)',
      color: '#333333'
    });
  }

  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}
