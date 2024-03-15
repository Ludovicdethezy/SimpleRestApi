using hostlink;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static SimpleRestApi.Orders;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Data;
using System.ComponentModel.DataAnnotations;


namespace SimpleRestApi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    { 
       
        public static string WIP_host = "localhost:8018";// ip of fleetcontroler 
        public static string ip_host_webhooks = "http://+:8001/order/";// ip of fleetcontroler 
        private bool test2;
        public static  BindingList<Orderrow>   _orderlist;
        public static string carwash_location ="";
        Thread c;

        public MainWindow()
        {
            InitializeComponent();
           
            _orderlist= new BindingList<Orderrow>();
            order_list.ItemsSource = _orderlist;

            GetOrder();
            carwash_location = carwash.Text;

        }



        //public class Custom_dataC
        //{
        //    public string? LoadWeight { get; set; }
        //    public string? LoadDimensionX { get; set; }
        //    public string? LoadDimensionY { get; set; }
        //    public string? FetchHeight { get; set; }
        //    public string? DeliverHeight { get; set; }

        //    public string? AGV { get; set; }
        //    public string? Area { get; set; }

        //    public int? Sequence { get; set; }

        //}
       // public  TransportOrderDefinition NewcarwahTO()
       // {

       //     TransportOrderStep stp1 = new TransportOrderStep
       //     {
       //         Operation_type = "Drop",
       //         Addresses = new string[] { carwash.Text }
       //     };


       //     Custom_dataC custom = new Custom_dataC
       //     {
       //         LoadWeight = "0",
       //         LoadDimensionX = "0",
       //         LoadDimensionY = "0",
       //         FetchHeight = "0",
       //         DeliverHeight = "0",
              
       //     };

       //     var to = new TransportOrderDefinition
       //     {
       //         Transport_order_id = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString(),
       //         Transport_unit_type = "Pallet",
       //         Start_time = null,
       //         End_time = DateTimeOffset.UtcNow.AddMinutes(10),
       //         Custom_data = custom,
       //         Steps = new TransportOrderStep[] { stp1 },
       //         Partial_steps = false,
       //     };
       //     return to;
       //}


            public static  void Addorder(string orderid, string fetch, string deliver, string date, string agv, string step, string status)
        {
            var ord = new Orderrow
            {
                orderid = orderid,
                Fetch = fetch,
                Deliver = deliver,
                Date = date,
                AGV = agv,
                step = step,
                Status = status

            };
            _orderlist.Add(ord);
        }

        public static void UpdateOrder(string orderid, string date, string agv, string step, string status)
        {
           

            var ord = _orderlist.FirstOrDefault(s => s.orderid == orderid);
            if (ord != null)
            {
                ord.Date = date;
                ord.AGV = agv;
                ord.step= step;
                ord.Status = status;
            }
         
        
            }

        public void GetOrder()
        {

            try
            {
                HttpClient client2 = new HttpClient();
                var sys = new Client(client2);
                sys.BaseUrl = "http://" + WIP_host + "/api/v1/";
                string[] ad = new string[4];
                int i = 0;
                var x = sys.OrdersAllAsync().GetAwaiter().GetResult();

                foreach (TransportOrderDefinition item in x)
                {

                    foreach (TransportOrderStep stp in item.Steps)
                    {

                        ad[i] = stp.Addresses.First();
                        i++;
                    }
                    Addorder(item.Transport_order_id, ad[0], ad[1], item.End_time.ToString(), "x", "", "");
                   // order_list.Items.Add(new { OrderId = item.Transport_order_id, Fetch = ad[0], Deliver = ad[1], Date = item.End_time.ToString(), AGV = "x" });

                }
            }
            catch (Exception ex)
            {
                WriteMessage(ex.Message, Brushes.Red, logview);
            }
        }


            private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient client2 = new HttpClient();
                var sys = new Client(client2);
                string deliver2 = deliver.Text;
                string fetch2 = fetch.Text;
                string AG = AGV.Text;

                sys.BaseUrl = "http://" + WIP_host + "/api/v1/";

                // create step for order : step0 fetch 
                TransportOrderStep stp0 = new TransportOrderStep
                {
                    Operation_type = "Pick",
                    Addresses = new string[] { fetch.Text },
                    Constraint_group_id = "",
                    Constraint_group_index = 0
                };

                // create step for order : step1 deliver
                TransportOrderStep stp1 = new TransportOrderStep
                {
                    Operation_type = "Drop",
                    Addresses = new string[] { deliver.Text },
                   // Constraint_group_id = null,
                    //Constraint_group_index = 0

                };


                // create data for custome data  like AGV number , fetch /deliver height , or anything else relevent for the order to work 
               Custom_dataC custom = new Custom_dataC
                {

                    AGV = AGV.Text,
                    Area = "",
                    Sequence = 0
                };

                // create order with all previous data unique id 
                var to = new TransportOrderDefinition
                {
                    Transport_order_id = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString(),
                    Transport_unit_type = "Pallet",
                    //    Start_time =null,
                    End_time = DateTimeOffset.UtcNow.AddMinutes(20),
                    Custom_data = custom,
                    Steps = new TransportOrderStep[] { stp0, stp1 },
                    Partial_steps = false,
                };

                
                Addorder(to.Transport_order_id, fetch2, deliver2, to.End_time.ToString(), "x", "", "");
                var x2 = sys.Orders2Async(to, to.Transport_order_id).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteMessage(ex.ToString(), Brushes.Red, logview);
            }


        }

        // show log 
        public  void WriteMessage(string message, Brush color, ListView lv)
        {

            Dispatcher.BeginInvoke(new Action(delegate
            {
                ListViewItem ls = new ListViewItem
                {
                    Foreground = color,
                    Content = message
                };
                lv.Items.Add(ls);
                lv.ScrollIntoView(lv.Items[lv.Items.Count - 1]);
            }));
        }

        private  void Updateord()
        {

            Dispatcher.BeginInvoke(new Action(delegate
            {
                order_list.ItemsSource = _orderlist;
                CollectionViewSource.GetDefaultView(order_list.ItemsSource).Refresh();
            }));
        }

        private void Eventreceive_ack_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client2 = new HttpClient();
            var sys = new Client(client2);
            string status ;
            string agv = "0";
           
            sys.BaseUrl = "http://" + WIP_host + "/api/v1/";
            try
            {

               
                    var x = sys.EventsAllAsync(10, true, 0, null).GetAwaiter().GetResult();
                
                    foreach (Event item in x)
                    {
                       


                        if (item.Type == "AgvOperationEndEvent")
                        {

                            item.Payload.AdditionalProperties.TryGetValue("address", out object adress);
                            item.Payload.AdditionalProperties.TryGetValue("transport_order_id", out object id);
                            item.Payload.AdditionalProperties.TryGetValue("step_index", out object step);
                            item.Payload.AdditionalProperties.TryGetValue("drive_start_time", out object start);
                            item.Payload.AdditionalProperties.TryGetValue("error_name", out object error);
                            item.Payload.AdditionalProperties.TryGetValue("end_status", out object status_op);
                            //Console.WriteLine(" validation adress: {0},transport_order_id : {1} , step {2} ", value, value2, value3);
                            string er;
                            if (error == null)
                                er = "";
                            else er = error.ToString();


                        WriteMessage(" - t_id : " + id.ToString() + " waiting ack for adresse: " + adress.ToString() + ", step: " + step.ToString() + "  ,status: " + status_op.ToString() + "  ,error: " + er, Brushes.Red, logview);
                 
                                          
                         
                              sys.ContinueAsync(id.ToString(), item.Event_id);
                                Console.WriteLine(" ack operation envoyé ");

                        WriteMessage(" * t_id : " + id.ToString() + " ack sent for adresse: " + adress.ToString() + ", step: " + step.ToString(), Brushes.Blue, logview);


                        if (step.ToString() == "0")
                        {

                            if (status_op.ToString() == "Success")
                            {
                                status = "Pallet fetched";
                               
                            }
                            else
                            {
                                status = status_op.ToString();
                            }
                            UpdateOrder(id.ToString(), start.ToString(), agv, step.ToString(), status.ToString());
                            WriteMessage(" * t_id : " + id.ToString() + "  : adress:" + adress.ToString() + ", step: " + step.ToString() + " " + status, Brushes.Red, logview);

                        }
                        else
                        {
                            if (status_op.ToString() == "Success")
                            {
                                status = "Pallet delivered";
                              
                            }


                            else
                            {
                                status = status_op.ToString();
                               
                            }

                            UpdateOrder(id.ToString(), start.ToString(), agv, step.ToString(), status.ToString());
                            WriteMessage(" * t_id : " + id.ToString() + " finish : adress:" + adress.ToString() + ", step: " + step.ToString() + " "+ status, Brushes.Red, logview);

                        }




                    }
                    if (item.Type == "AgvArrivedToAddressEvent")
                    {
                        item.Payload.AdditionalProperties.TryGetValue("address", out object adress);
                        item.Payload.AdditionalProperties.TryGetValue("transport_order_id", out object id);
                            item.Payload.AdditionalProperties.TryGetValue("step_index", out object step);
                            item.Payload.AdditionalProperties.TryGetValue("drive_start_time", out object start);
                            

                          
                                     WriteMessage(" - t_id : " + id.ToString() + " waiting preload ack : adress: \" " + adress.ToString() + ", step: " + step.ToString() , Brushes.Red, logview);


                        sys.ContinueAsync(id.ToString(), item.Event_id);

                        WriteMessage(" * t_id : " + id.ToString() + " preload ack sent : adress:" + adress.ToString() + ", step: " + step.ToString(), Brushes.Red, logview);

                       



                    }
                    if (item.Type == "UnconnectedOrderCreatedEvent")
                        {
                            Console.WriteLine("carwash");

                        var tcwo = new TransportOrderDefinition();
                        tcwo = Orders.NewcarwahTO();
                        var sys2 = new Order(client2);
                        try
                        {
                            var x2 = sys2.AckAsync(item.Event_id, tcwo).GetAwaiter().GetResult();
                            WriteMessage(" carwash ", Brushes.Red, logview);
                        }
                        catch
                        {
                            Console.WriteLine(" Problem with carwash  ");

                        }


                      
                    }
                    if (item.Type == "ParameterUpdateEvent")
                    {
                        Console.WriteLine("event update");
                        item.Payload.AdditionalProperties.TryGetValue("parameter_name", out object name);
                        item.Payload.AdditionalProperties.TryGetValue("transport_order_id", out object To_id);
                        item.Payload.AdditionalProperties.TryGetValue("parameter_value", out object value);
                        //   item.Payload.AdditionalProperties.TryGetValue("drive_start_time", out object value4);




                        ParameterRequestAnswer stp1 = new ParameterRequestAnswer
                        {
                            Event_id = item.Event_id,
                            Parameter_name = name.ToString(),
                            Parameter_value = value.ToString()
                        };

                        HttpClient client = new HttpClient();

                        var syspar = new Param(client);

                        syspar.BaseUrl = "http://" + WIP_host + "/api/v1/";
                        syspar.ParAckAsync(To_id.ToString(), stp1);

                        WriteMessage(" * t_id : " + To_id.ToString() + " parameter:" + name.ToString() + ", value: " + value.ToString(), Brushes.Orange, logview);


                    }
                }


                
            }
            catch (System.InvalidOperationException ex)
            {
                Console.WriteLine(ex);

            }
            catch
            {
                Console.WriteLine(" Problem with ack ");

            }

           
        }

        private async void Start_web_Click(object sender, RoutedEventArgs e)
        {
            
            
            if (test2)
            {
                try
                {

                  
                    string url = ip_host_webhooks.Replace("+", "localhost");

                    HttpClient client = new HttpClient();
                    var requestData = new { type = "stop" };
                    string json = System.Text.Json.JsonSerializer.Serialize(requestData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    //MessageBox.Show(responseContent);*
                    test2 = false;
                    // test.IsEnabled = true;
                    start_web.Content = "start Webhook \n listening ";
                    start_web.Background = Brushes.GreenYellow;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    WriteMessage("webhook: " + ex.Message, Brushes.MediumPurple, logview);
                    // test.IsEnabled = true;
                    test2 = false;

                }
            }
            else
            {
                ip_host_webhooks = Webhook_adress.Text;
                start_web.Content = "stop Webhook \n listening ";
                start_web.Background = Brushes.OrangeRed;
                WriteMessage("Test: start Listening on " + Webhook_adress.Text, Brushes.MediumPurple, logview);
                test2 = true;
                c = new Thread(ServerAsync);
                c.IsBackground = true;
                c.Start();
            }
        }

        private void ServerAsync()
        {
            try
            {


                var listener = new HttpListener();

                listener.Prefixes.Add(ip_host_webhooks);

                listener.Start();


                Console.WriteLine("Listening on " + ip_host_webhooks);

                while (true)
                {



                    HttpListenerContext ctx = listener.GetContext();
                    HttpListenerRequest request = ctx.Request;
                    // Console.WriteLine($"Received request for {request.Url}");
                    string ua = request.Headers.Get("User-Agent");
                    Console.WriteLine($"{request.HttpMethod} {request.Url}");

                    var body = request.InputStream;
                    var encoding = request.ContentEncoding;
                    var reader = new StreamReader(body, encoding);
                    string s = "null";
                    bool ok = true;
                    if (request.HasEntityBody)
                    {


                        if (request.ContentType != null)
                        {
                            Console.WriteLine("Client data content type {0}", request.ContentType);
                        }
                        Console.WriteLine("Client data content length {0}", request.ContentLength64);

                        // Console.WriteLine("Start of data:");
                        s = reader.ReadToEnd();
                        //Console.WriteLine(s);
                        //Console.WriteLine("End of data:");
                        // var jsonTextReader = new Newtonsoft.Json.JsonTextReader(reader);

                        //Event Event2 = System.Text.Json.JsonSerializer.Deserialize<Event>(s);


                        var dyna = JsonConvert.DeserializeObject<dynamic>(s);
                        string type = dyna.type;
                        if (type == "stop")
                        {
                            var response2 = ctx.Response;
                            response2.StatusCode = (int)HttpStatusCode.OK;
                            response2.ContentType = "text/plain";
                            response2.OutputStream.Write(new byte[] { }, 0, 0);
                            response2.OutputStream.Close();
                            break;
                        }




                        Webhook.webhook_receive(dyna, WIP_host);
                        WriteMessage("Webhooks: " + type.ToString() + " " + s, Brushes.Green, logview);
                        //Dispatcher.Invoke(new InvokeDelegate(() =>
                        //{

                        //    ListViewItem li = new ListViewItem();
                        //    li.Foreground = Brushes.Green;
                        //    li.Content = "Webhooks: " + type.ToString() + " " + s;
                        //    this.log2.Items.Add(li);
                        //    this.log2.SelectedItem = this.log2.Items.Count;
                        //    webh.Background = Brushes.LightGreen;


                        //}));
                        //  refresh_ioDG();
                        // refresh_orderDG();
                        reader.Close();
                        body.Close();

                    }
                    var response = ctx.Response;

                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/plain";
                    response.OutputStream.Write(new byte[] { }, 0, 0);
                    response.OutputStream.Close();
                    Updateord();
        

                    }
                listener.Stop();
                Console.WriteLine("Stop Listening on port 8001...");

                WriteMessage("  Webhook : " + "Stop Listening on" + ip_host_webhooks, Brushes.Blue, logview);

            }
            catch (HttpListenerException ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

       
    }    
}
