namespace TripWebData.Inputs;

/// <summary>
/// 旅游产品图片输入参数
/// </summary>
public class TravelImageInput
{
    /// <summary>
    /// 大图片地址
    /// </summary>
    public string BigImg { get; set; } = null!;
    /// <summary>
    /// 小图片地址
    /// </summary>
    public string? SmallImg { get; set; }
}