using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WhatSAP.Models
{
    public class SearchResultModel
    {
        public DateTime Date { get; set; }

        public string Keyword { get; set; }

        [Range(0, 100, ErrorMessage = "Price must be betweeen $0.00 and $100.00")]
        public double Price { get; set; }

        public long CategoryId { get; set; }

        public long typeId { get; set; }

        public List<Activity> ActivityResults { get; set; }

        public SearchResultModel()
        {
            Date = DateTime.Now;
            Price = 0;
            CategoryId = 1;
            Keyword = "";

            ActivityResults = new List<Activity>();
        }
    }
}
