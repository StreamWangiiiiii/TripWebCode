using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripWebData.Entity
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public long Id { get; set; }
        /// <summary>
        /// 添加人
        /// </summary>
        [Column("created_user_id")]
        public long CreatedUserId { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [Column("created_time")]
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 更新人
        /// </summary>
        [Column("updated_user_id")]
        public long UpdatedUserId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("updated_time")]
        public DateTime? UpdatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否删除
        /// </summary>
        [Column("deleted")]
        public bool Deleted { get; set; }
    }
}
