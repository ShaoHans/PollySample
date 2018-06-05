using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Polly;
using Polly.Fallback;

namespace PollyTest1
{
    public static class Fallback_回退
    {
        public static void Test()
        {
            try
            {
                string result = Policy<string>.Handle<ArgumentNullException>()
                        .Fallback<string>("匿名")
                        .Execute(() =>
                        {
                            return GetUserName(null);
                        });
                Console.WriteLine($"返回值：{result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常信息：{ex.Message}");
            }
        }

        public static void Test2()
        {
            FallbackPolicy<string> policy = Policy<string>.Handle<ArgumentNullException>()
                .Fallback("匿名");

            try
            {
                for (int i = 0; i < 3; i++)
                {
                    string result = policy.Execute(() =>
                    {
                        return GetUserName(i == 0 ? null : (int?)i);
                    });
                    Console.WriteLine($"用户名是：{result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常信息：{ex.Message}");
            }
        }

        public static string GetUserName(int? userId)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            if(userId.HasValue)
            {
                return $"Tom{userId}";
            }
            else
            {
                throw new ArgumentNullException("用户Id不能为空");
            }
        }
    }

}
