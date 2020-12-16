import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { request } from 'https';
import { ToastrService } from 'ngx-toastr';
import { Observable, throwError } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

    constructor(private router: Router, private toastr: ToastrService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      return next.handle(req).pipe(
        catchError(error => {

          if (error) {

            if (error.status == 400) {
              // if error 400 has list of errors this means this is validation error 
              if (error.error.errors) {
                throw error.error;
              }
              else {
                this.toastr.error(error.error.message, error.error.statusCode);
              }
            }
            if (error.status == 401) {
              this.toastr.error(error.error.message, error.error.statusCode);
            }
            if (error.status === 404) {
              this.router.navigateByUrl('/not-found');
            }
            // if we have got an error status of 500 then what we will do is pass the exception information to our
            // server error component
            if (error.status === 500) {
              const navigationExtras: NavigationExtras = { state: {error: error.error} }
              this.router.navigateByUrl('/server-error', navigationExtras);
            }

          }
          return throwError(error);

        })
      );
    }
    

}
