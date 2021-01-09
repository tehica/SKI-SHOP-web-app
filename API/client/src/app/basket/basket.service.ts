import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { error } from 'protractor';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Basket, IBasket, IBasketItem, IBasketTotals } from '../shared/models/Basket';
import { IProduct } from '../shared/models/product';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root'
})
export class BasketService {

  baseUrl = environment.apiUrl;

  // because this is behaviour subject then it's always going to emit value how many items are added in basket
  private basketSource = new BehaviorSubject<IBasket>(null);  // $ sign is because this is observable variable
  basket$ = this.basketSource.asObservable();

  private basketTotalSource = new BehaviorSubject<IBasketTotals>(null);
  basketTotal$ = this.basketTotalSource.asObservable();

  shipping = 0;

  constructor(private http: HttpClient) { }

  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    this.shipping = deliveryMethod.price;
    const basket = this.getCurrentBasketValue();
    basket.deliveryMethodId = deliveryMethod.id;
    basket.shippingPrice = deliveryMethod.price;
    this.calculateTotals();
    this.setBasket(basket);
  }

  getBasket(id: string) {
    // we want from response when we get back from the HTTP Client which should contain basket
    // then we need to set basketSource with the basket we get back from the API
    // we wil achieve this with .pipe()
    return this.http.get(this.baseUrl + 'basket?id=' + id)
      .pipe(
        map((basket: IBasket) => {
          this.basketSource.next(basket);
          // after we've got the baskets from the API we can go ahead and set the totals
          // so call calculateTotals() method down there
          this.calculateTotals();
        })
      );
  }

  // post 
  setBasket(basket: IBasket) {
    return this.http.post(this.baseUrl + 'basket', basket).subscribe((response: IBasket) => {
      this.basketSource.next(response);
      this.calculateTotals();
    }, error => {
        console.log(error);
    });
  }


  getCurrentBasketValue() {
    return this.basketSource.value;
  }

  addItemToBasket(item: IProduct, quantity = 1) {
    const itemToAdd: IBasketItem = this.mapProductItemToBasketItem(item, quantity);

    // if getCurrentBasketvalue method returns null then call createBasket method 
    let basket = this.getCurrentBasketValue();
    if (basket === null) {
      basket = this.createBasket();
    }

    basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);

    this.setBasket(basket);
  }

  incrementItemQuantity(item: IBasketItem) {
    // get current basket as it stands
    const basket = this.getCurrentBasketValue();
    // check to see if there is an existing item of this type in the baskets already (item what is passed in method)
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    basket.items[foundItemIndex].quantity++;
    this.setBasket(basket);
  }

  decrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    // check if the quantity that we currently have is greater than 1 and if it is decrement quantity by 1
    // or if quantity is 1 then call removeItemFromBasket method and pass that BasketItem
    if (basket.items[foundItemIndex].quantity > 1) {
      basket.items[foundItemIndex].quantity--;
      this.setBasket(basket);
    } else {
      this.removeItemFromBasket(item);
    }
  }

  removeItemFromBasket(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    // some() returns boolean and it checks if there is any item with this id
    if (basket.items.some(x => x.id === item.id)) {
      // basket.items.filter(i => i.id !== item.id); this will return an array, this will populate basket items
      // except for the one that matches this id (i => i.id !== item.id)
      basket.items = basket.items.filter(i => i.id !== item.id);
      // if this was the only item remaining in the basket and we remove them
      // now we have empty basket and we will delete this basket
      if (basket.items.length > 0) {
        this.setBasket(basket);
      } else {
        this.deleteBasket(basket);
      }
    }
  }

  deleteLocalBasket(id: string) {
    this.basketSource.next(null);
    this.basketTotalSource.next(null);
    localStorage.removeItem('basket_id');
  }

  // at this point we're going to go up to API and remove it from there as well
  deleteBasket(basket: IBasket) {
    return this.http.delete(this.baseUrl + 'basket?id=' + basket.id).subscribe(() => {
      this.basketSource.next(null);
      this.basketTotalSource.next(null);
      // remove the key with the basket
      localStorage.removeItem('basket_id');
    }, error => {
      console.log(error);
    });
  }


  // method for calculate the totals from what's inside basket and add them to basketTotalSource
  private calculateTotals() {
    const basket = this.getCurrentBasketValue();
    const shipping = this.shipping;
    // b - represents the item and each item has a price and the quantity
    // then this calculation sum with 'a'
    // a - represents the number of results that we are returning from this produce function
    // for a is given a initial value of zero (..., 0)
    const subtotal = basket.items.reduce((a, b) => (b.price * b.quantity) + a, 0);
    const total = subtotal + shipping;

    this.basketTotalSource.next({ shipping, total, subtotal });
  }

  // method who check to see if there is already an item of this type in a basket and if there is
  // then just increase and set quantity
  private addOrUpdateItem(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
    const index = items.findIndex(i => i.id === itemToAdd.id);

    // if index is not found
    if (index === -1) {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    } else {
      items[index].quantity += quantity;
    }

    return items;
  }


  private createBasket(): IBasket {
    // create new basket with unique identifier for this basket ( random string )
    // and it will be also basket with empty list of items as well
    const basket = new Basket();

    // we will use local storage on the client's browser to store the baskets id once it's created
    // it will allow us (if user doenst't clear their local storage) to retrieve their baskets
    // even if they cloed their browser or restart their computer
    localStorage.setItem('basket_id', basket.id);

    return basket;
  }

  // method for map properties between IProduct and IBasketItem
  private mapProductItemToBasketItem(item: IProduct, quantity: number): IBasketItem {
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      quantity,
      brand: item.productBrand,
      type: item.productType
    };
  }
}
