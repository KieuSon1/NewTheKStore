using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewTheKStore.Controllers
{
    public class Cart
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public decimal price { get; set; }
        public int count { get; set; }

        public Cart(int? id, string name, string url, decimal price, int count)
        {
            this.id = id;
            this.name = name;
            this.url = url;
            this.price = price;
            this.count = count;
        }

        public override string ToString()
        {
            return id + "|" + name + "|" + url + "|" + price + "|" + count;
        }
    }
}