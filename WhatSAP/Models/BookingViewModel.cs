using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhatSAP.Models
{
    public class BookingViewModel
    {
        public long BookingId { get; set; }
        public Activity Activity { get; set; }
        public Customer Customer { get; set; }
        public int NumOfPpl { get; set; }
        public double Total { get; set; }
        public DateTime BookingDate { get; set; }
        public bool Confirmed { get; set; }

        public BookingViewModel() {
            Activity = new Activity();
            Customer = new Customer();
        }

        public BookingViewModel(long id, Activity activity, Customer customer, int num, double total, DateTime bookingDate, bool confirmed)
        {
            BookingId = id;
            Activity = activity;
            Customer = customer;
            NumOfPpl = num;
            Total = total;
            BookingDate = bookingDate;
            Confirmed = confirmed;
        }
    }
}
