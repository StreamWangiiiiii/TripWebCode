using System.ComponentModel.DataAnnotations;

namespace TripWebData.Inputs;

/// <summary>
/// 上架与下架景点
/// </summary>
public class TravelShelveOrUnShelveInput:BaseInput
{
    /// <summary>
    /// 待下架的景点列表
    /// </summary>
    [Required] public List<long> TravelIdList { get; set; }
    
    /// <summary>
    /// 是否上架
    /// </summary>
    [Required] public bool HasShelve { get; set; }
}