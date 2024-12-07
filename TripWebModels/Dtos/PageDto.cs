namespace TripWebData.Dtos;

/// <summary>
/// 分页-输出对象
/// </summary>
public class PageDto<T>
{
    /// <summary>
    /// 总条数
    /// </summary>
    public int Total { get; set; }

    public List<T> Data { get; set; }
}