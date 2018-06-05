using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Polly;
using Polly.CircuitBreaker;

namespace PollyTest1
{
    public static class CircuitBreaker_熔断器
    {
        /// <summary>
        /// 自动熔断
        /// </summary>
        public static void AutoCircuit()
        {
            Action<Exception, CircuitState, TimeSpan, Context> onBreak = (ex, state, ts, context) =>
            {
                Console.WriteLine($"熔断{ts.Seconds}秒，当前状态：{state.ToString()}，异常信息：{ex.Message}");
            };

            Action<Context> onReset = context =>
            {
                Console.WriteLine($"熔断器恢复");
            };

            Action onHalfOpen = () =>
            {

            };

            //当发生2次Exception的异常的时候则会熔断5秒钟，该操作后续如果继续尝试执行则会直接返回错误 。
            var policy = Policy.Handle<Exception>()
                .CircuitBreaker(2, TimeSpan.FromSeconds(5), onBreak, onReset, onHalfOpen);

            while (true)
            {
                try
                {
                    if(policy.CircuitState == CircuitState.HalfOpen || policy.CircuitState == CircuitState.Open)
                    {
                        Console.WriteLine("熔断器处于打开状态，不执行Execute方法");
                    }
                    bool b = policy.Execute(() =>
                            {
                                Console.WriteLine("开始执行");
                                bool result = DoSomething();
                                Console.WriteLine("执行完成");
                                return result;
                            });

                    if (b)
                        break;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"熔断器执行过程中出现异常：{ex.Message}");
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

        }

        public static bool DoSomething()
        {
            int currentSeconds = DateTime.Now.Second;
            if (currentSeconds % 5 == 0)
            {
                Console.WriteLine($"当前秒数{currentSeconds}能被5整除");
                return true;
            }
            else
            {
                throw new Exception($"当前秒数{currentSeconds}不能被5整除");
            }
        }
    }
}
