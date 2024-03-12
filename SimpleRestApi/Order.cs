using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRestApi
{
    public class Orders
    {
        public class Orderrow
        {
            public string orderid { get; set; }
            public string Fetch { get; set; }
            public string Deliver{ get; set; }
            public string Date { get; set; }
            public string AGV { get; set; }
            public string step { get; set; }
            public string Status { get; set; }
        }

        
    }
}
