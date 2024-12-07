namespace TripWebData.Consts;

/// <summary>
/// 即时通讯用到的方法名
/// </summary>
public class SignalMethod
{
    /// <summary>
    ///  客户端事先定义好的回调方法，用于接收服务端返回过来的消息
    /// </summary>
    public const string ReceiveMessage = nameof(ReceiveMessage);

    /// <summary>
    /// 批量导入用户的进度条
    /// </summary>
    public const string ImportUserProcess = nameof(ImportUserProcess);
}