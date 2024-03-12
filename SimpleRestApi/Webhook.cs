using hostlink;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRestApi
{
    class Webhook
    {
        public static void webhook_receive(object dyna,string WIP_host)
        {


            try
            {


                dynamic ev = JsonConvert.DeserializeObject<dynamic>(dyna.ToString());
                string type = ev.type;
                string Ev_id = ev.event_id;
                string ack = ev.waiting_acknowledgement;
                string To_id = "";
                string start_time = "";
                string error = "";
                string end_reason = "";
                string end_status = "";
                string created = "";

                HttpClient client2 = new HttpClient();
                var sys = new Client(client2);
                //  sys.BaseUrl = "http://" + MainWindow.IP_host + "/api/v1/";
                
                    sys.BaseUrl = "http://" + WIP_host + "/api/v1/";
             

                if (ev.payload != null)
                {
                    To_id = ev.payload.transport_order_id;
                    start_time = ev.payload.drive_start_time;
                    error = ev.payload.error_name;
                    end_reason = ev.payload.end_reason;
                    end_status = ev.payload.end_status;
                    created = ev.created_at;

                }
                switch (type)
                {
                    case var s when type.Contains("AgvOperationEndEvent"):
                        {
                            Console.WriteLine("wait ack postload");
                            string status;
                            DateTimeOffset crea = ev.created_at;
                            DateTimeOffset startd = ev.payload.drive_start_time;
                            string step = ev.payload.step_index;

                            if ( ack == "False")
                            {
                                if (step == "0")

                                    if (end_status == "Success")
                                    {
                                        status = "AGV waiting fetched ack";
                                        //MainWindow.updatedb(To_id, DateTimeOffset.Parse(start_time), status, int.Parse(Ev_id));
                                       MainWindow.UpdateOrder(To_id, startd.ToString(),"0" ,step,status);
                                    }
                                    else
                                    {
                                        status = "AGV waiting ack " + end_status.ToString();

                                        //MainWindow.updatedb_error(To_id, DateTimeOffset.Parse(start_time), status, error, int.Parse(Ev_id));
                                        // MainWindow.updatedb_error(To_id, startd, status, error, int.Parse(Ev_id));
                                        MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);

                                    }

                                else


                                if (end_status == "Success")
                                {
                                    status = "AGV waiting delivered ack";
                                    //MainWindow.updateendoforder(To_id, DateTimeOffset.Parse(created), status, int.Parse(Ev_id));
                                    // MainWindow.updateendoforder(To_id, crea, status, int.Parse(Ev_id));
                                    MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);
                                }


                                else
                                {
                                    status = "AGV waiting ack " + end_status.ToString();
                                    //MainWindow.updateendoforder_error(To_id, DateTimeOffset.Parse(start_time), status, error, int.Parse(Ev_id));
                                    //   MainWindow.updateendoforder_error(To_id, startd, status, error, int.Parse(Ev_id));
                                    MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);
                                }

                            }

                            else
                            {
                                sys.ContinueAsync(To_id, int.Parse(Ev_id));

                                if (step == "0")
                                {

                                    if (end_status == "Success")
                                    {
                                        status = "Pallet fetched";
                                        //MainWindow.updatedb(To_id, DateTimeOffset.Parse(start_time), status, int.Parse(Ev_id));
                                        //  MainWindow.updatedb(To_id, startd, status, int.Parse(Ev_id));
                                        MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);
                                    }
                                    else
                                    {
                                        status = end_status.ToString();
                                        // MainWindow.updatedb_error(To_id, DateTimeOffset.Parse(start_time), status, error, int.Parse(Ev_id));
                                        //  MainWindow.updatedb_error(To_id, startd, status, error, int.Parse(Ev_id));
                                        MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);
                                    }


                                }
                                else
                                {
                                    if (end_status == "Success")
                                    {
                                        status = "Pallet delivered";
                                        // MainWindow.updateendoforder(To_id, DateTimeOffset.Parse(created), status, int.Parse(Ev_id));
                                        //   MainWindow.updateendoforder(To_id, crea, status, int.Parse(Ev_id));
                                        MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);
                                    }


                                    else
                                    {
                                        status = end_status.ToString();
                                        // MainWindow.updateendoforder_error(To_id, DateTimeOffset.Parse(start_time), status, error, int.Parse(Ev_id));
                                        //  MainWindow.updateendoforder_error(To_id, startd, status, error, int.Parse(Ev_id));
                                        MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);
                                    }
                                }
                            }

                            //MainWindow.updatedb(To_id, DateTimeOffset.Parse(start_time), "");
                            break;
                        }
                    case var s when type.Contains("AgvArrivedToAddressEvent"):
                        {
                            Console.WriteLine("wait ack preload");
                            string status;
                            string step = ev.payload.step_index;
                            DateTimeOffset startd = ev.payload.drive_start_time;

                            if ( ack == "False")
                            {
                                if (step == "0")
                                    status = "AGV waiting ack to fetch";
                                else
                                    status = "AGV waiting ack to deliver";
                                MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);
                                // MainWindow.updatedb(To_id, DateTimeOffset.Parse(start_time.ToString()), status, int.Parse(Ev_id));
                                // MainWindow.updatedb(To_id, startd, status, int.Parse(Ev_id));

                            }
                            else
                            {
                                sys.ContinueAsync(To_id, int.Parse(Ev_id));

                                if (step == "0")
                                    status = "AGV ready to fetch";
                                else
                                    status = "AGV ready to deliver";
                                MainWindow.UpdateOrder(To_id, startd.ToString(), "0", step, status);
                                // MainWindow.updatedb(To_id, DateTimeOffset.Parse(start_time.ToString()), status, int.Parse(Ev_id));
                                // MainWindow.updatedb(To_id, startd, status, int.Parse(Ev_id));
                            }

                            break;
                        }
                    case var s when type.Contains("ParameterQueryEvent"):
                        {
                            Console.WriteLine("wait parameter");
                            // string status;
                            string name = ev.payload.parameter_name;
                            string value = " ";
                            //  DateTimeOffset startd = ev.payload.drive_start_time;
                            ParameterRequestAnswer stp1 = new ParameterRequestAnswer
                            {
                                Event_id = int.Parse(Ev_id),
                                Parameter_name = name,
                                Parameter_value = value
                            };

                            HttpClient client = new HttpClient();

                            //var syspar = new ParameterClient(client);
                            //if (MainWindow.usew == true)
                            //    syspar.BaseUrl = "http://" + MainWindow.WIP_host + "/api/v1/";
                            //else
                            //    syspar.BaseUrl = "http://" + MainWindow.IP_host + "/api/v1/";
                            //syspar.ParAsync(To_id, stp1);



                            break;
                        }
                    case var s when type.Contains("ParameterUpdateEvent"):
                        {
                            Console.WriteLine("wait parameter");
                            // string status;
                            string name = ev.payload.parameter_name;
                            string value = ev.payload.parameter_value;
                            //  DateTimeOffset startd = ev.payload.drive_start_time;
                            ParameterRequestAnswer stp1 = new ParameterRequestAnswer
                            {
                                Event_id = int.Parse(Ev_id),
                                Parameter_name = name,
                                Parameter_value = value
                            };

                            HttpClient client = new HttpClient();
                            //var syspar = new ParameterClient(client);
                            //if (MainWindow.usew == true)
                            //    syspar.BaseUrl = "http://" + MainWindow.WIP_host + "/api/v1/";
                            //else
                            //    syspar.BaseUrl = "http://" + MainWindow.IP_host + "/api/v1/";

                            //syspar.ParAsync(To_id, stp1);



                            break;
                        }

                    case var s when type.Contains("SystemStartupEvent"):
                        {
                            //MainWindow.insert_agv();
                            break;
                        }
                    case var s when type.Contains("TransportOrderEndEvent"):
                        {
                            Console.WriteLine("end event");
                            if (end_reason == "Failed")
                            {
                                Console.WriteLine(end_reason.ToString());

                            }
                            else
                            {



                                //string stcustom = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)ev.payload.custom_data).First).Value).Value.ToString();
                                //if (stcustom == "io_request")
                                //{
                                //    foreach (var item in (Newtonsoft.Json.Linq.JContainer)ev.payload.custom_data)
                                //    {
                                //        if (((Newtonsoft.Json.Linq.JProperty)item).Value.ToString() == "io_request")
                                //        {

                                //        }

                                //        else
                                //        {


                                //        }


                                //    }
                                //}
                                //else
                                //{
                                DateTimeOffset crea = ev.created_at;
                                //MainWindow.updateendoforder(To_id, crea, end_reason, int.Parse(Ev_id));
                           
                            }
                            // MainWindow.refresh_ioDG();
                            break;
                        }
                    case var s when type.Contains("CustomInformationEvent"):
                        {

                            //string stcustom = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)ev.payload.custom_data).First).Value).Value.ToString();
                            //if (stcustom == "input_update")
                            //{
                            //    string connectionString = MainWindow.Connect_string;
                            //    try
                            //    {
                            //        using (SqlConnection connection2 = new SqlConnection(connectionString))
                            //        {
                            //            connection2.Open();

                            //            //status_db.Content = "Data updated to db";
                            //            SqlCommand cmd = new SqlCommand("UPDATE IO SET  IO_Value = @IO_Value ,IO_Type = @IO_Type, IO_State = @IO_State Where  IO_Name = @IO_Name ", connection2);

                            //            cmd.Parameters.AddWithValue("@IO_Type", "input_wu");
                            //            cmd.Parameters.AddWithValue("@IO_Value", ev.payload.custom_data.Input_val.ToString());
                            //            cmd.Parameters.AddWithValue("@IO_Name", ev.payload.custom_data.Input_nam.ToString());
                            //            cmd.Parameters.AddWithValue("@IO_State", ev.payload.custom_data.Input_sta.ToString());
                            //            Console.WriteLine(ev.payload.custom_data.Input_val.ToString() + "  " + ev.payload.custom_data.Input_nam);

                            //            cmd.ExecuteNonQuery();



                            //            connection2.Close();
                            //        }
                            //    }
                            //    catch (SqlException ex)
                            //    {
                            //        Console.WriteLine("ERREUR DE CONNEXION ...");
                            //        Console.WriteLine(ex.Message.ToString());
                            //        // WriteLog("ERREUR DE CONNEXION ... " + ex.Message);

                            //        throw ex;
                            //    }


                            //}
                            break;
                        }

                    case var s when type.Contains("AgvState"):
                        {
                         
                            




                           

                            break;
                        }

                    case var s when type.Contains("UnconnectedOrderCreatedEvent"):
                        {
                        //    var tcwo = new TransportOrderDefinition();
                        //    tcwo = MainWindow.NewcarwahTO();
                        //    var sys2 = new OrderClient(client2);
                         
                        //        sys2.BaseUrl = "http://" + MainWindow.WIP_host + "/api/v1/";
                           
                        //    try
                        //    {
                        //        var x2 = sys2.AckAsync(int.Parse(Ev_id), tcwo).GetAwaiter().GetResult();
                        //    }
                        //    catch
                        //    {
                        //        Console.WriteLine(" Problem with carwash  ");

                        //    }
                            break;
                        }

                    case var s when type.Contains("OrderState"):
                        {
                            string order_id = ev.data.id;
                            string est_finished_time = ev.data.estimated_finish_time;
                            // DateTimeOffset fin= ev.data.estimated_finish_time;
                            // string agvs = ev.data.current_agvs[].Lenght;
                            int agv = 0;
                            if (((Newtonsoft.Json.Linq.JArray)ev.data.current_agvs).Count != 0)
                                agv = (int)ev.data.current_agvs.First.Value;
                            string csutom = JsonConvert.SerializeObject(ev.data.custom_fields);
                            if (est_finished_time != null)
                            {
                                DateTimeOffset fin = ev.data.estimated_finish_time;
                             //   MainWindow.updateorder(order_id, fin, agv, 0, csutom);
                            }

                            //MainWindow.updateorder(order_id, DateTimeOffset.Parse(est_finished_time), agv,0);

                            break;
                        }

                    default:
                        {
                            Console.WriteLine("default");
                            break;
                        }
                }


            }
            catch (ApiException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            {
            }
        }

    }
}
