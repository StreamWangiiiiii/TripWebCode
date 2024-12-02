using System.Text.Json.Serialization;

namespace Travel.Data.Inputs;

/// <summary>
/// 旅游景点添加或者修改-输入参数
/// </summary>
public class TravelAddOrUpdateInput:BaseInput
{
    /// <summary>
        /// 主键，大于0-修改，等于0-添加
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string TravelName { get; set; } = null!;
        /// <summary>
        /// 价格
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 旅游线路介绍
        /// </summary>
        public string? Introduce { get; set; }
        // /// <summary>
        // /// 1-已上架,0-未上架
        // /// </summary>
        // public bool IsLaunched { get; set; }
        // /// <summary>
        // /// 上架时间
        // /// </summary>
        // public DateTime? LaunchedTime { get; set; }
        // /// <summary>
        // /// 是否主题旅游
        // /// </summary>
        // public bool IsThemeTravel { get; set; }
        /// <summary>
        /// 所属分类
        /// </summary>
        public long? CategoryId { get; set; }
        /// <summary>
        /// 目的地省份
        /// </summary>
        public long? ProvinceId { get; set; }
        /// <summary>
        /// 目的地省份名称，冗余字段
        /// </summary>
        public string ProvinceName { get; set; } = null!;
        /// <summary>
        /// 目的地城市
        /// </summary>
        public long? CityId { get; set; }
        /// <summary>
        /// 目的地城市名称
        /// </summary>
        public string? CityName { get; set; }
        /// <summary>
        /// 目的地具体地址
        /// </summary>
        public string? DetailAddress { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string? ThumbImgage { get; set; }
        // /// <summary>
        // /// 收藏次数
        // /// </summary>
        // public int? FavoriteCount { get; set; }
        // /// <summary>
        // /// 审核状态：1-通过，0-驳回，2-待审核
        // /// </summary>
        // public short? AuditStatus { get; set; }
        /// <summary>
        /// 出发时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 人数上限
        /// </summary>
        public int? PersonLimit { get; set; }
        
        
        /// <summary>
        /// 旅游图片列表
        /// </summary>
        public List<TravelImageInput> ImageList { get; set; }
}