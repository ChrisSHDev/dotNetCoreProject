using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhatSAP.Models
{
    public class CartViewModel
    {
        public int NumOfPeople { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
