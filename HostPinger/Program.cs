using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.NetworkInformation;

namespace HostPinger
{
    class Program
    {
        static void Main(string[] args)
        {
            Pinger pinger = new Pinger();
            pinger.Run();

        }
    }

    class Pinger
    {
        //string Host_URL = "43.226.6.156";// you may change this to new server address 
        string Host_URL = "43.226.6.156";
        string action_choice = "P";
        public void Run()
        {
            Console.WriteLine("\n\tHi! Welcome To HostPinger :)  By Danilo Alingasa \t\t\tServer/Address: " + this.Host_URL);
            //UnComment three lines below if you want to input dynamic server / address
            /* 
             * Console.Write("\n\tInput URL to Ping: ");
               this.Host_URL = Console.ReadLine();
               bool res = PingHost("43.226.6.156");
            */
           
            do
            {
                this.Start(this.Host_URL);
            } while (action_choice != "X" && action_choice != "x");
            Console.WriteLine("\n");
        }

        public void Start(string URL)
        {
            bool pingable = PingHost(URL);
            if (!pingable)
            {
                Console.WriteLine("\n\tsending email to server.....");
                if (SendEmail(URL))
                {
                    Console.WriteLine("\n\temail successfully sent.....");
                }
                else
                {
                    Console.WriteLine("\n\temail sent failed.....");
                }
            }
            else
            {
                Console.WriteLine("\n\t>>>>>>> " + URL + " is currently stable/ON :)");
            }
            Console.WriteLine("\n");
            Console.Write("\n\tEnter [X]-To Exit, [ANY KEY]-Ping again :");
            this.action_choice = Console.ReadLine();
            //Thread.Sleep(3000);
        }

        public bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            int counter = 0;
            Ping pinger = null;

            do
            {
                try
                {
                    pinger = new Ping();
                    PingReply reply = pinger.Send(nameOrAddress);
                    pingable = reply.Status == IPStatus.Success;
                    Console.WriteLine("\n\t>>>>>>> Ping attempt no. " + (counter + 1) + "\t\t Status: " + reply.Status + " >>>>>>>");
                    if (reply.Status.ToString() == "Success")
                    {
                        counter = 20;
                    }
                    else
                    {
                        counter++;
                    }
                }
                catch (PingException)
                {
                    // Discard PingExceptions and return false;
                    Console.WriteLine("\n\t>>>>>>> Ping attempt no. " + (counter + 1) + "\t\t Status: Failed >>>>>>>");
                    counter++;
                }
                finally
                {
                    if (pinger != null)
                    {
                        pinger.Dispose();
                    }
                }
            } while (counter < 20);

            return pingable;
        }


        public bool SendEmail(string url)
        {
            try
            {
                var message = "There is a failure in the IP " + url; // Modify this message
                var status = "failed";
                var computerName = Environment.MachineName.ToString();

                // Computer IP Addresses
                //foreach(var el in Dns.GetHostAddresses(Environment.MachineName))
                //{
                //    Console.WriteLine(el.ToString());
                //}

                // Don't change this script it use to send Email
                var APIBaseURL = "https://script.google.com/macros/s/AKfycbwHvGMjTNO0H1BGH2mo2JvvBbQCmkbs9KOk50iAWoBg2YtyL3w/exec";
                using (var webclient = new WebClient())
                {
                    byte[] response = webclient.UploadValues(APIBaseURL, new NameValueCollection()
                    {
                        {"Server",this.Host_URL },
                        {"Computer",computerName},
                        {"Attempt","20" },
                        {"from","PINGER" },
                    });
                    string result = System.Text.Encoding.UTF8.GetString(response);
                    var jsonObject = JObject.Parse(result);
                    status = jsonObject["result"].ToString();
                }
                if (status == "success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("\n\tUnable to send Email!.Make sure you have internet access!.");
                return false;
            }
        }
    }
}
