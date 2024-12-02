using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Travel.Data.Inputs;

/// <summary>
/// 旅游景点审批
/// </summary>
public class TravelAuditInput:BaseInput
{
    /// <summary>
    /// 待审批的景点ID
    /// </summary>
    [Required] public List<long> TravelIdList { get; set; }

    /// <summary>
    /// 是否通过
    /// </summary>
    [Required]public bool HasPass { get; set; }
    
}