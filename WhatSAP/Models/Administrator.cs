using System;
using System.Collections.Generic;

namespace WhatSAP.Models
{
    public partial class Administrator
    {
        public long AdminId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
