using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Polly;

namespace PollyTest1
{
    public static class Retry_重试
    {
        /// <summary>
        /// 重试一次
        /// </summary>
        public static void RetryByCount(int count = 1)
        {
            try
            {
                Policy.Handle<Exception>()
                        .Retry(count, (ex, retryCount) =>
                        {
                            Console.WriteLine($"重试第{retryCount}次，异常信息：{ex.Message}");
                        })
                        .Execute(() =>
                        {
                            DoSomething();
                        });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        /// <summary>
        /// 不断重试，直至成功
        /// </summary>
        public static void RetryForever()
        {
            int retryCount = 0;
            Policy.Handle<Exception>()
                .RetryForever((ex, context) =>
                {
                    retryCount++;
                    Console.WriteLine($"重试第{retryCount}次，异常信息：{ex.Message}");
                })
                .Execute(() =>
                {
                    DoSomething();
                });
        }

        /// <summary>
        /// 等待重试
        /// </summary>
        public static void WaitAndRetry(int count = 1)
        {
            try
            {
                if (count == 1)
                {
                    Policy.Handle<Exception>()
                    .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3)
                    }, (ex, ts) =>
                     {
                         Console.WriteLine($"{ts.Seconds}秒后的重试结果：{ex.Message}");
                     })
                    .Execute(() =>
                    {
                        DoSomething2();
                    });
                }
                else
                {
                    Policy.Handle<Exception>()
                        .WaitAndRetry(count, (retryCount) =>
                         {
                             return TimeSpan.FromSeconds(retryCount);
                         }, (ex, ts, retryCount, context) =>
                          {
                              Console.WriteLine($"第{retryCount}次{ts.Seconds}秒后的重试结果：{ex.Message}");
                          })
                        .Execute(() =>
                        {
                            DoSomething2();
                        });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"等待重试失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 等待重试直到成功
        /// </summary>
        public static void WaitAndRetryForever()
        {
            Policy.Handle<Exception>()
                .WaitAndRetryForever((retryCount) =>
                {
                    return TimeSpan.FromSeconds(1);
                }, (ex, ts) =>
                 {
                     Console.WriteLine($"{ts.Seconds}秒后的重试结果：{ex.Message}");
                 })
                .Execute(() =>
                {
                    DoSomething2();
                });
        }

        private static int GenerateSeed()
        {
            byte[] bytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static void DoSomething()
        {
            int number = new Random(GenerateSeed()).Next(1, 42);
            if (number % 21 == 0)
            {
                Console.WriteLine($"获取到了能被21整除的随机数{number}");
            }
            else
            {
                throw new Exception($"错误的随机数{number}");
            }
        }

        public static void DoSomething2()
        {
            int currentSeconds = DateTime.Now.Second;
            if(currentSeconds % 35 == 0)
            {
                Console.WriteLine($"当前秒数{currentSeconds}能被35整除");
            }
            else
            {
                throw new Exception($"当前秒数{currentSeconds}不能被35整除");
            }
        }
    }
}
