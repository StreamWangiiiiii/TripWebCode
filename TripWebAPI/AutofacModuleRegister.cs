using Autofac;
using System.Reflection;
using TripWebService;

namespace TripWebAPI
{
    /// <summary>
    /// Autofac注册服务类
    /// </summary>
    public class AutofacModuleRegister : Autofac.Module
    {
        /// <summary>
        /// 注册业务相关的服务至IOC容器中
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TokenService>();

            // 批量注册服务
            builder.RegisterAssemblyTypes(Assembly.Load("TripWebService"))
            .Where(a => a.Name.EndsWith("Service")).AsImplementedInterfaces();
        }
    }
}
