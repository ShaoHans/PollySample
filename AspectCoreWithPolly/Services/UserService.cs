using AspectCoreWithPolly.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspectCoreWithPolly.Services
{
    public class UserService
    {
        [HystrixCommand(nameof(GetGoogleIndexHtmlFallBackAsync))]
        public virtual async Task<string> GetGoogleIndexHtmlAsync()
        {
            //HttpClient httpClient = new HttpClient();
            //string result = await httpClient.GetStringAsync("https://www.google.com");
            //return result;
            int i = 1, j = 0;
            int k = i / j;
            return k.ToString();
        }

        public async Task<string> GetGoogleIndexHtmlFallBackAsync()
        {
            return await Task.FromResult<string>("访问谷歌首页失败");
        }

    }
}
