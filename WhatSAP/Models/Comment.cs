using System;
using System.Collections.Generic;

namespace WhatSAP.Models
{
    public partial class Comment
    {
        public long CommentId { get; set; }
        public string Body { get; set; }
        public decimal? Rate { get; set; }
        public long? CustomerId { get; set; }
        public long? ActivityId { get; set; }

        public Activity Activity { get; set; }
        public Customer Customer { get; set; }
    }
}
