using Core.Entities;
using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);

        // this method will handle the payment succeeded
        Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);

        // this method will handle the payment failed
        Task<Order> UpdateOrderPaymentFailed(string paymentIntentId);

    }
}
