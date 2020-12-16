import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IBrand } from '../shared/models/brand';
import { IPagination } from '../shared/models/pagination';
import { IType } from '../shared/models/productType';
import { map } from 'rxjs/operators';
import { ShopParams } from '../shared/models/shopParams';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})


export class ShopService {

  baseUrl = "https://localhost:44303/api/";

  // place where is http service injected
  constructor(private http: HttpClient) {}

  // method for return products
  getProduct(shopParams: ShopParams) {

    // HttpParams are parameters in URL for example: url?brandId=10
    let params = new HttpParams();

    if (shopParams.brandId !== 0) {
      params = params.append('brandId', shopParams.brandId.toString());
    }

    if (shopParams.typeId !== 0) {
      params = params.append('typeId', shopParams.typeId.toString());
    }

    if (shopParams.search) {
      params = params.append('search', shopParams.search);
    }

    params = params.append('sort', shopParams.sort);
    params = params.append('pageIndex', shopParams.pageNumber.toString());
    params = params.append('pageIndex', shopParams.pageSize.toString());

    // pipe allows us to chain multiple js operators to manipulate or do something with the observable as it comes back in.
    return this.http.get<IPagination>(this.baseUrl + 'products', { observe: 'response', params })
      .pipe(
        map(response => {
          return response.body;
        })
      );
  }

  getProductDetails(id: number) {
    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  // method for return brands
  getBrands() {
    return this.http.get<IBrand[]>(this.baseUrl + 'products/brands');
  }

  // method for return types
  getTypes() {
    return this.http.get<IType[]>(this.baseUrl + 'products/types');
  }
}
