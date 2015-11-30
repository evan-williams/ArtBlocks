using System;
using System.IO;
using LTI.API.Client.Helpers;

namespace LTI.API.Client.Jobs
{
    internal class RawRequest
    {
        internal void Post()
        {
            var gateway = APIHelper.GetTradevineGateway();
            var request = File.ReadAllText(@"Resources\RawRequest.json");
            const string url = "v1/SalesOrder";
            var rawResponse = gateway.Raw.ManualPost(url, request);
            Console.WriteLine("Raw response: {0}", rawResponse);
        }

    }
}
