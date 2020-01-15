using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SampleProject
{
    public class DataContext:DbContext
    {
        public int id { get; set; }
        public string VideoName { get; set; }
        public Nullable<int> NoOfView { get; set; }
        public Nullable<System.DateTime> ViewDate { get; set; }
        public Nullable<int> VideoId { get; set; }
    }
}