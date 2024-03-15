using hostlink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static SimpleRestApi.MainWindow;

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


        public class Custom_dataC
        {
            public string LoadWeight { get; set; }
            public string LoadDimensionX { get; set; }
            public string LoadDimensionY { get; set; }
            public string FetchHeight { get; set; }
            public string DeliverHeight { get; set; }

            public string AGV { get; set; }
            public string Area { get; set; }

            public int Sequence { get; set; }

        }


        public static TransportOrderDefinition NewcarwahTO()
        {

            TransportOrderStep stp1 = new TransportOrderStep
            {
                Operation_type = "Drop",
                Addresses = new string[] { carwash_location }
            };


            Custom_dataC custom = new Custom_dataC
            {
                LoadWeight = "0",
                LoadDimensionX = "0",
                LoadDimensionY = "0",
                FetchHeight = "0",
                DeliverHeight = "0",

            };

            var to = new TransportOrderDefinition
            {
                Transport_order_id = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString(),
                Transport_unit_type = "Pallet",
                Start_time = null,
                End_time = DateTimeOffset.UtcNow.AddMinutes(10),
                Custom_data = custom,
                Steps = new TransportOrderStep[] { stp1 },
                Partial_steps = false,
            };
            return to;
        } 
        
     
        }
   

}
