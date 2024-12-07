namespace TripWebData;

/// <summary>
/// 保存ServiceProvider
/// </summary>
public static class LocalServiceProvider
{
    /// <summary>
    /// IServiceProvider的实例
    /// </summary>
    public static IServiceProvider? Instance { get; set; }
}