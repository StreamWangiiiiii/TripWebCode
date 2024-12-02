using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Travel.Data.Inputs;

/// <summary>
/// 根据景点ID，查询旅客的输入参数
/// </summary>
public class TravelUserQueryInput
{
    /// <summary>
    /// 旅游景点ID
    /// </summary>
    [Required] public long TravelId { get; set; }

    // /// <summary>
    // /// 如果是导出，则不执行分页查询
    // /// </summary>
    // [JsonIgnore]
    // public bool Export { get; set; } = false;
}