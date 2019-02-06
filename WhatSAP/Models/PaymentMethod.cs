using System;
using System.Collections.Generic;

namespace WhatSAP.Models
{
    public partial class PaymentMethod
    {
        public long MethodCode { get; set; }
        public long CustomerId { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public DateTime Expiration { get; set; }
        public string Cvv { get; set; }

        public Customer Customer { get; set; }
    }
}
