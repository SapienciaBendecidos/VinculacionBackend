﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VinculacionBackend.Models
{
    public class HourEntryModel
    {
        public string AccountId { get; set; }

        public long SectionId { get; set; }

        public long ProjectId{ get; set; }
       
        public int Hour { get; set; }
    }
}