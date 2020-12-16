using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities.OrderAggregate
{
    public enum OrderStatus
    {
        // to get string value from enum member
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Payment Recived")]
        PaymentRecived,

        [EnumMember(Value = "Payment Failed")]
        PaymentFailed
    }
}
