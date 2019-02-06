using System;
using System.Collections.Generic;

namespace WhatSAP.Models
{
    public partial class Category
    {
        public Category()
        {
            Activity = new HashSet<Activity>();
        }

        public long CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<Activity> Activity { get; set; }
    }
}
