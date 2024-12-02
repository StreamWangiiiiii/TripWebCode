using System.ComponentModel.DataAnnotations;

namespace Travel.Data.Inputs;

/// <summary>
/// 为某个旅游景点设置领队输入参数
/// </summary>
public class SetTravelLeaderInput:BaseInput
{
    /// <summary>
    /// 景点ID
    /// </summary>
    [Required]public long TravelId { get; set; }

    /// <summary>
    /// 游客ID列表(领队ID)
    /// </summary>
    [Required] public List<long> UserIdList { get; set; } = null!;
}