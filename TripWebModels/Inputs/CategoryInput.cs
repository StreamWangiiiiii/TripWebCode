using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Travel.Data.Inputs;

/// <summary>
/// 分类入参
/// </summary>
public class CategoryInput:BaseInput
{
    /// <summary>
    /// 大于 0-修改，等于 0 添加
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    [Required]
    public string CategoryName { get; set; } = null!;
    
}