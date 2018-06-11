using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore;
using AspectCore.DynamicProxy;

namespace AspectCoreWithPolly.Attributes
{
    public class HystrixCommandAttribute : AbstractInterceptorAttribute
    {
        public string FallBackMethodName { get; private set; }

        public HystrixCommandAttribute(string fallBackMethodName)
        {
            FallBackMethodName = fallBackMethodName;
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
    }
}
