using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WhatSAP.Models.Cart
{
    public class PaymentViewModel
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        [MaxLength(128)]
        [Display(Name = "Name on Card")]
        public string NameOnCard { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime Expiration { get; set; }

        [Required]
        [MaxLength(3)]
        public string CVV { get; set; }
    }
}
