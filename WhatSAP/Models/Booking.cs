using System;
using System.Collections.Generic;

namespace WhatSAP.Models
{
    public partial class Booking
    {
        public long BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public double Total { get; set; }
        public long? CustomerId { get; set; }
        public long? ClientId { get; set; }
        public long? ActivityId { get; set; }
        public long? PaymentId { get; set; }
        public bool Confirmed { get; set; }
        
        public Activity Activity { get; set; }
        public Client Client { get; set; }
        public Customer Customer { get; set; }
    }
}
