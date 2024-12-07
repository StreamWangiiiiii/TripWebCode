namespace TripWebData.Dtos.TravelBusiness;

/// <summary>
/// 收藏夹输出参数
/// </summary>
public class FavoriteDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 旅游产品ID
    /// </summary>
    public long TravelId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// 景点名称
    /// </summary>
    public string TravelName { get; set; } = null!;

    /// <summary>
    /// 景点价格
    /// </summary>
    public double Price { get; set; }
    
    /// <summary>
    /// 收藏时间
    /// </summary>
    public DateTime CreatedTime { get; set; }
}