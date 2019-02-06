using System;
using System.Collections.Generic;

namespace WhatSAP.Models
{
    public partial class Payment
    {
        public long PaymentId { get; set; }
        public long CustomerId { get; set; }
        public long MethodCode { get; set; }
        public DateTime PaymentDate { get; set; }
        public double PaymentAmount { get; set; }
        public double DiscountAmount { get; set; }

        public Customer Customer { get; set; }
    }
}
