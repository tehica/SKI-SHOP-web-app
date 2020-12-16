import { v4 as uuidv4 } from 'uuid'

export interface IBasket {
  id: string;
  items: IBasketItem[];
}

export interface IBasketItem {
  id: number;
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand: string;
  type: string;
}

// whenever we create a new instance of the basket its going to have a unique identifier ans it's gonna have
// an empty array of items
export class Basket implements IBasket {
    id = uuidv4();
    items: IBasketItem[] = [];

}


export interface IBasketTotals {
  shipping: number;
  subtotal: number;
  total: number;
}
