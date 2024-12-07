namespace TripWebData.Inputs;

/// <summary>
/// 角色权限分配-输入参数
/// </summary>
public class RoleAuthorityInput:BaseInput
{
    /// <summary>
    /// 给哪个角色分配权限
    /// </summary>
    public long RoleId { get; set; }
    
    /// <summary>
    /// 给这个权限分配哪些权限(按钮)
    /// </summary>
    public List<long> ButtonIdList { get; set; }
}