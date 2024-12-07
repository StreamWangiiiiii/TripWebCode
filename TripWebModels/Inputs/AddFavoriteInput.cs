using System.Text.Json.Serialization;
using TripWebData.Inputs;

namespace TripWebData.Inputs;

/// <summary>
/// 添加收藏夹-输入参数
/// </summary>
public class AddFavoriteInput : BaseInput
{
    /// <summary>
    /// 旅游景点ID
    /// </summary>
    public long TravelId { get; set; }

}