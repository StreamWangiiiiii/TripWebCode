using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripWebData.Dtos.TravelBusiness
{
    public class CategoryDto
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; } = null!;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdatedTime { get; set; }

        /// <summary>
        /// 最后更新人
        /// </summary>
        public long? UpdatedUserId { get; set; }

        /// <summary>
        /// 最后更新人名称
        /// </summary>
        public string? UpdatedUserName { get; set; }
    }
}
