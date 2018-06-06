using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace PollyTest1
{
    public class Bulkhead_舱壁隔离
    {
        public static void Test()
        {
            var policy = Policy.Bulkhead(5, (context) =>
             {
                 Console.WriteLine("请求量太大");
             });

            for (int i = 0; i < 1000; i++)
            {
                Task.Run(() =>
                {
                    policy.Execute(() =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        Console.WriteLine("方法执行完成");
                    });
                });
            }
            Console.WriteLine("over");
        }
    }
}
