using LTI.API.Model.Client;
using LTI.Services.Concrete.Live.Implementation.Rate_Limiting;

namespace LTI.API.Client.Helpers
{
    public class APIHelper
    {
        public static TradevineGateway GetTradevineGateway()
        {
            var client = new TradevineClient(
                Config.Current.ConsumerKey,
                Config.Current.ConsumerSecret,
                Config.Current.APIAuthority,
                Config.Current.RequestTokenUrl,
                Config.Current.AccessTokenUrl,
                Config.Current.AuthoriseUrl,
                Config.Current.AccessTokenKey,
                Config.Current.AccessTokenSecret,
                new RateLimiterUnrestricted());

            return new TradevineGateway(client);
        }
    }
}
