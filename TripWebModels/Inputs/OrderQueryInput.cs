using System.Text.Json.Serialization;

namespace TripWebData.Inputs;

/// <summary>
/// 订单查询输入参数
/// </summary>
public class OrderQueryInput:PageInput
{
    /// <summary>
    /// 旅游景点
    /// </summary>
    public string? TravelName { get; set; }
    /// <summary>
    /// 下单时间-开始区间
    /// </summary>
    public DateTime? StartTime { get; set; }
    /// <summary>
    /// 下单时间-结束区间
    /// </summary>
    public DateTime? EndTime { get; set; }
    /// <summary>
    /// 订单所属商家
    /// </summary>
    [JsonIgnore]
    public long? SellerUserId { get; set; }
    /// <summary>
    /// 当前登录人员
    /// </summary>
    [JsonIgnore]
    public long? LoginUserId { get; set; }
}