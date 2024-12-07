namespace TripWebData.Dtos.TravelBusiness;

public class TravelDto
{
        /// <summary>
        /// 主键
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
        /// 1-已上架,0-未上架
        /// </summary>
        public bool IsLaunched { get; set; }
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime? LaunchedTime { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; } = null!;
        /// <summary>
        /// 目的地省份名称，冗余字段
        /// </summary>
        public string ProvinceName { get; set; } = null!;
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
        /// <summary>
        /// 收藏次数
        /// </summary>
        public int? FavoriteCount { get; set; }
        /// <summary>
        /// 审核状态：1-通过，0-驳回，2-待审核
        /// </summary>
        public short? AuditStatus { get; set; }
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
        /// 最后更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; }

        /// <summary>
        /// 所属商家
        /// </summary>
        public string? SellerUserName { get; set; }

        /// <summary>
        /// 所属商家
        /// </summary>
        public long CreatedUserId { get; set; }
}