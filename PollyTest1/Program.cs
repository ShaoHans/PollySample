using System;

namespace PollyTest1
{
    class Program
    {
        /*
         * 参考文章 http://www.jessetalk.cn/2018/03/25/asp-vnext-polly-docs/
         *         https://tech.meituan.com/service-fault-tolerant-pattern.html
         *         在进行系统设计、特别是进行分布式系统设计的时候以“Design For Failure”（为失败而设计）为指导原则
        Polly 错误处理使用三步曲
            1.定义条件： 定义你要处理的错误异常或者返回结果
            2.定义处理方式 ： 重试，熔断，回退
            3.执行 
            伪代码如下：
                // 这个例子展示了当DoSomething方法执行的时候如果遇到SomeExceptionType的异常则会进行重试调用。
                var policy = Policy
                  .Handle<SomeExceptionType>() // 定义条件 
                  .Retry(); // 定义处理方式
                policy.Execute(() => DoSomething()); // 执行
        =======================================================================================
        在Polly中，对这些服务容错模式分为两类：
            错误处理fault handling（当错误已经发生时）：重试（Retry）、熔断器（Circuit Breaker）、回退（Fallback）
            弹性应变resilience（错误发生前）：超时（Timeout）、舱壁隔离（Bulkhead Isolation）、缓存（Cache）、限流（Rate Limiting）
        */
        static void Main(string[] args)
        {
            /*
            Retry_重试.RetryByCount(3);
            Console.WriteLine("=========================================");
            Retry_重试.RetryForever();
            Console.WriteLine("=========================================");
            Retry_重试.WaitAndRetry();
            Console.WriteLine("=========================================");
            Retry_重试.WaitAndRetry(5);
            Console.WriteLine("=========================================");
            Retry_重试.WaitAndRetryForever();
            */

            /*
            CircuitBreaker_熔断器.AutoCircuit();
            */

            /*
            Fallback_回退.Test();
            Fallback_回退.Test2();
            */

            //Timeout_超时.Timeout_Fallback();
            Console.WriteLine("=========================================");
            Timeout_超时.Timeout_Retry();

            Console.ReadKey();
        }
    }
}
