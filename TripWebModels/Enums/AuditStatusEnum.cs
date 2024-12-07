using System.ComponentModel;

namespace TripWebData.Enums;

/// <summary>
/// 审核状态：1-通过，0-驳回，2-待审核
/// </summary>
public enum AuditStatusEnum
{
    /// <summary>
    /// 驳回
    /// </summary>
    [Description("驳回")]
    Refuse=0,
    /// <summary>
    /// 通过
    /// </summary>
    [Description("通过")]
    Pass=1,
    /// <summary>
    /// 待审核
    /// </summary>
    [Description("待审核")]
    UnAudit=2
}