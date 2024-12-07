namespace TripWebData.Dtos.TravelBusiness;

/// <summary>
/// 查看某个旅游景点所有报名人员信息
/// </summary>
public class QueryTravelUserDto
{
    /// <summary>
    /// 领队列表
    /// </summary>
    public List<LeaderUserDto> LeaderList { get; set; } = new();

    /// <summary>
    /// 旅游景点所有报名人员分页数据
    /// </summary>
    public List<TravelUserDto> UserList { get; set; } = new();
}

/// <summary>
/// 领队输出参数
/// </summary>
public class LeaderUserDto
{
    /// <summary>
    /// 领队姓名
    /// </summary>
    public string LeaderNickname { get; set; } = null!;
    /// <summary>
    /// 领队手机
    /// </summary>
    public string LeaderMobile { get; set; } = null!;
}