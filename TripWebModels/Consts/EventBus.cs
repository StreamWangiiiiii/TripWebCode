namespace TripWebData.Consts;

/// <summary>
/// MQ 事件总线
/// </summary>
public class EventBus
{
    /// <summary>
    /// 创建订单的事件
    /// </summary>
    public const string OrderAdd = "order.add";
    
    /// <summary>
    /// 批量导入用户的事件
    /// </summary>
    public const string ImportUser = "import.user";
}