using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace IndependentReserve.Common.Tools
{
    public static class HttExtension
    {
        public static async Task<string> GetAsync(this HttpClient client, Uri uri, NameValueCollection inParams = null)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            if (inParams != null)
            {
                parameters.Add(inParams);
            }

            var uriBuilder = new UriBuilder(uri)
            {
                Query = parameters.ToString()
            };

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri.AbsoluteUri);
            using var response = await client.SendAsync(requestMessage).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
