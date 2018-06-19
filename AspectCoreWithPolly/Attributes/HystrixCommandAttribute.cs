using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore;
using AspectCore.DynamicProxy;
using Polly;

namespace AspectCoreWithPolly.Attributes
{
    public class HystrixCommandAttribute : AbstractInterceptorAttribute
    {
        public string FallBackMethodName { get; private set; }
        public Guid TestId { get; set; }
        public HystrixCommandAttribute(string fallBackMethodName)
        {
            FallBackMethodName = fallBackMethodName;
            TestId = Guid.NewGuid();
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                await context.Invoke(next);
            }
            catch (Exception ex)
            {
                var fallBackMethod = context.ServiceMethod.DeclaringType.GetMethod(FallBackMethodName);
                object fallBackResult = fallBackMethod.Invoke(context.Implementation, context.Parameters);
                context.ReturnValue = fallBackResult;
                await Task.FromResult(0);
            }
        }

        /*
        private void test()
        {
            //一个HystrixCommand中保持一个policy对象即可
            //其实主要是CircuitBreaker要求对于同一段代码要共享一个policy对象
            //根据反射原理，同一个方法就对应一个HystrixCommandAttribute，无论几次调用，
            //而不同方法对应不同的HystrixCommandAttribute对象，天然的一个policy对象共享
            //因为同一个方法共享一个policy，因此这个CircuitBreaker是针对所有请求的。
            //Attribute也不会在运行时再去改变属性的值，共享同一个policy对象也没问题
            lock (this)//因为Invoke可能是并发调用，因此要确保policy赋值的线程安全
            {
                if (policy == null)
                {
                    policy = Policy
                            .Handle<Exception>()
                            .FallbackAsync(async (ctx, t) =>
                            {
                                AspectContext aspectContext = (AspectContext)ctx["aspectContext"];
                                var fallBackMethod = context.ServiceMethod.DeclaringType.GetMethod(this.FallBackMethod);
                                Object fallBackResult = fallBackMethod.Invoke(context.Implementation, context.Parameters);
                                //不能如下这样，因为这是闭包相关，如果这样写第二次调用Invoke的时候context指向的
                                //还是第一次的对象，所以要通过Polly的上下文来传递AspectContext
                                //context.ReturnValue = fallBackResult;
                                aspectContext.ReturnValue = fallBackResult;
                            }, async (ex, t) => { });
                    if (MaxRetryTimes > 0)
                    {
                        policy = policy.WrapAsync(Policy.Handle<Exception>().WaitAndRetryAsync(MaxRetryTimes, i => TimeSpan.FromMilliseconds(RetryIntervalMilliseconds)));
                    }
                    if (EnableCircuitBreaker)
                    {
                        policy = policy.WrapAsync(Policy.Handle<Exception>().CircuitBreakerAsync(ExceptionsAllowedBeforeBreaking, TimeSpan.FromMilliseconds(MillisecondsOfBreak)));
                    }
                    if (TimeOutMilliseconds > 0)
                    {
                        policy = policy.WrapAsync(Policy.TimeoutAsync(() => TimeSpan.FromMilliseconds(TimeOutMilliseconds), Polly.Timeout.TimeoutStrategy.Pessimistic));
                    }
                }
            }
            //把本地调用的AspectContext传递给Polly，主要给FallbackAsync中使用，避免闭包的坑
            Context pollyCtx = new Context();
            pollyCtx["aspectContext"] = context;
            //Install-Package Microsoft.Extensions.Caching.Memory
            if (CacheTTLMilliseconds > 0)
            {
                //用类名+方法名+参数的下划线连接起来作为缓存key
                string cacheKey = "HystrixMethodCacheManager_Key_" + context.ServiceMethod.DeclaringType
                + "." + context.ServiceMethod + string.Join("_", context.Parameters);
                //尝试去缓存中获取。如果找到了，则直接用缓存中的值做返回值
                if (memoryCache.TryGetValue(cacheKey, out var cacheValue))
                {
                    context.ReturnValue = cacheValue;
                }
                else
                {
                    //如果缓存中没有，则执行实际被拦截的方法
                    await policy.ExecuteAsync(ctx => next(context), pollyCtx);
                    //存入缓存中
                    using (var cacheEntry = memoryCache.CreateEntry(cacheKey))
                    {
                        cacheEntry.Value = context.ReturnValue;
                        cacheEntry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMilliseconds(CacheTTLMilliseconds);
                    }
                }
            }
            else//如果没有启用缓存，就直接执行业务方法
            {
                await policy.ExecuteAsync(ctx => next(context), pollyCtx);
            }

        }
        */
    }
}
