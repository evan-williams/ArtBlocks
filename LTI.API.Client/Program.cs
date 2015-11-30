using System;
using System.Net;
using System.Threading.Tasks;
using LTI.API.Client.Jobs;

namespace LTI.API.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var checkAts = new CheckAtsRegularly();

            try
            {  
                Task.Factory.StartNew(checkAts.Run);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception was thrown: {0} - {1}", ex.Message, ex.StackTrace);
                var webEx = ex as WebException;
                if (webEx != null)
                {
                    Console.WriteLine("");
                    var httpWebResponse = webEx.Response as HttpWebResponse;
                    Console.WriteLine(httpWebResponse.StatusDescription);
                }
            }
            finally
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                checkAts.Stop = true;
            }
        }
    }
}
