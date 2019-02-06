using System;
using System.Collections.Generic;

namespace WhatSAP.Models
{
    public partial class Address
    {
        public Address()
        {
            Activity = new HashSet<Activity>();
        }

        public long AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public ICollection<Activity> Activity { get; set; }
    }
}
