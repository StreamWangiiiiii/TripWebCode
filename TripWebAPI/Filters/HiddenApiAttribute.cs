using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TripWebAPI.Filters
{
    /// <summary>
    /// 隐藏接口，不生成到swagger文档展示
    /// 注意：如果不加[HiddenApi]标记的接口名称和加过标记的隐藏接口名称相同，则该普通接口也会被隐藏不显示，所以建议接口名称最好不要重复
    /// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property)]
    public class HiddenApiAttribute : Attribute { }
    /// <summary>
    /// 隐藏接口不生成到swagger文档展示
    /// </summary>
    public class HiddenApiFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (ApiDescription apiDescription in context.ApiDescriptions)
            {
                if (Enumerable.OfType<HiddenApiAttribute>
                (apiDescription.CustomAttributes()).Any())
                {
                    string key = "/" + apiDescription.RelativePath;
                    if (key.Contains("?"))
                    {
                        int idx = key.IndexOf("?", StringComparison.Ordinal);
                        key = key.Substring(0, idx);
                    }
                    swaggerDoc.Paths.Remove(key);
                    continue;
                }
            }
        }
    }
}
