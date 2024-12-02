using System.ComponentModel.DataAnnotations;

namespace Travel.Data.Inputs;

/// <summary>
/// 用户批量导入输入参数
/// </summary>
public class ImportUserInput
{
    /// <summary>
    /// 文件路径(必须以/开头)
    /// </summary>
    [Required]public string FilePath { get; set; } = null!;
    /// <summary>
    /// 当前登录用户
    /// </summary>
    public long LoginUserId { get; set; }
    /// <summary>
    /// SignalR 连接ID
    /// </summary>
    public string ConnectionId { get; set; }
}