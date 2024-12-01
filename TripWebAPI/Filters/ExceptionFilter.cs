using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TripWebData;

namespace TripWebAPI.Filters
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 当发生异常的时候会执行此方法
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnException(ExceptionContext context)
        {
            var values = context.RouteData.Values;
            var controller = values["controller"];
            var action = values["action"];
            _logger.LogError($"控制器：{controller},方法：{action},详细信息：\n");
            WriteDetailErrorMsg(context.Exception);
            context.Result = new JsonResult(Results<string>.FailResult(context.Exception.Message));
        }

        /// <summary>
        /// 递归获取内部异常信息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private void WriteDetailErrorMsg(Exception exception)
        {
            if (exception.InnerException != null)
            {
                _logger.LogError(exception.StackTrace + "\n\n");
                WriteDetailErrorMsg(exception.InnerException);
            }
        }
    }
}
