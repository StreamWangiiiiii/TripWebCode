namespace TripWebData.Dtos.TravelBusiness;

/// <summary>
/// 旅游产品图片列表对象
/// </summary>
public class TravelImageDto
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 旅游产品Id
    /// </summary>
    public long TravelId { get; set; }
    /// <summary>
    /// 大图片地址
    /// </summary>
    public string BigImg { get; set; } = null!;
    /// <summary>
    /// 小图片地址
    /// </summary>
    public string? SmallImg { get; set; }
}