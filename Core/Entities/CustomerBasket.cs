using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class CustomerBasket
    {
        // empty constructor is used for create a new instance without needing to know the Id
        public CustomerBasket()
        {
        }

        public CustomerBasket(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        // in thi prop will be stored delivery method which the user has selected in checkout process
        public int? DeliveryMethodId { get; set; }

        // this is going to be used by stripe so the user can confirm the payment intent
        public string ClientSecret { get; set; }

        public string PaymentIntentId { get; set; }

        public decimal ShippingPrice { get; set; }

    }
}
