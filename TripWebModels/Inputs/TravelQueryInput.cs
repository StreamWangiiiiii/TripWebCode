using System.Text.Json.Serialization;
using Travel.Data.Enums;

namespace Travel.Data.Inputs;

/// <summary>
/// 旅游查询-输入参数
/// </summary>
public class TravelQueryInput:PageInput
{
    /// <summary>
    /// 分类
    /// </summary>
    public long? CategoryId { get; set; }
    /// <summary>
    /// 发行时间查询区间-开始时间
    /// </summary>
    public DateTime? StartLaunchedTime { get; set; }
    /// <summary>
    /// 发行时间查询区间-结束时间
    /// </summary>
    public DateTime? EndLaunchedTime { get; set; }
    /// <summary>
    /// 标题
    /// </summary>
    public string? TravelName { get; set; }
    /// <summary>
    /// 价格查询区间-开始价格
    /// </summary>
    public double? StartPrice { get; set; }
    /// <summary>
    /// 价格查询区间-结束价格
    /// </summary>
    public double? EndPrice { get; set; }
    /// <summary>
    /// 1-已上架,0-未上架，null-查询所有
    /// </summary>
    public bool? IsLaunched { get; set; }
    /// <summary>
    /// 审核状态：1-通过，0-驳回，2-待审核,-1-查询所有
    /// </summary>
    public AuditStatusEnum? AuditStatus { get; set; }

    /// <summary>
    /// 是否为用户端查询，用户端只查询未开始的旅游景点
    /// </summary>
    [JsonIgnore]
    public bool IsUserSearch { get; set; } = false;
    
    /// <summary>
    /// 所属商家
    /// </summary>
    [JsonIgnore]
    public long CreatedUserId { get; set; }
}