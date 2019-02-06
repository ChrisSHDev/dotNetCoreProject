using System;
using System.Collections.Generic;

namespace WhatSAP.Models
{
    public partial class Client
    {
        public Client()
        {
            Activity = new HashSet<Activity>();
            Booking = new HashSet<Booking>();
        }

        public long ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<Activity> Activity { get; set; }
        public ICollection<Booking> Booking { get; set; }
    }
}
