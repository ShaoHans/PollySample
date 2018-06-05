using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Polly;
using Polly.Timeout;

namespace PollyTest1
{
    /// <summary>
    /// 超时策略一般都要和其他策略组合使用
    /// </summary>
    public static class Timeout_超时
    {
        /*
            超时分为乐观超时与悲观超时，乐观超时依赖于CancellationToken ，
            它假设我们的具体执行的任务都支持CancellationToken。那么在进行timeout的时候，
            它会通知执行线程取消并终止执行线程，避免额外的开销。
         */

        public static void Timeout_Fallback()
        {
            ISyncPolicy policy = Policy.Handle<Exception>()
                .Fallback(() =>
                {
                    Console.WriteLine("方法执行出错");
                });

            // 悲观超时
            // 悲观超时与乐观超时的区别在于，如果执行的代码不支持取消CancellationToken，它还会继续执行，这会是一个比较大的开销
            policy = policy.Wrap(Policy.Timeout(2, TimeoutStrategy.Pessimistic));
            policy.Execute(() =>
            {
                DoSomething();
            });
        }

        public static void Timeout_Retry()
        {
            var policy = Policy.Handle<TimeoutRejectedException>()
                .Retry(3, (ex, retryCount) =>
                 {
                     Console.WriteLine($"第{retryCount}次重试结果是{ex.Message}");
                 });
            var wrapper = policy.Wrap(Policy.Timeout(2, TimeoutStrategy.Pessimistic));
            try
            {
                 wrapper.Execute(() =>
                    {
                        Console.WriteLine("开始执行方法");
                        var result = Calc();
                        Console.WriteLine($"执行方法结束，返回结果：{result}");
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        public static bool Calc()
        {
            Thread.Sleep(2000);
            if (DateTime.Now.Second % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void DoSomething()
        {
            Console.WriteLine("执行一个需要花费4秒的方法...");
            Thread.Sleep(TimeSpan.FromSeconds(4));
            Console.WriteLine("方法执行完成");
        }
    }
}
