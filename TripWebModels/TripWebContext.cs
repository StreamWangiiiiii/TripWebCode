using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using TripWebData.Entity;

namespace TripWebData;

public partial class TripWebContext : DbContext
{
    public TripWebContext(DbContextOptions<TripWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<County> Counties { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<TabCategory> TabCategories { get; set; }

    public virtual DbSet<TabFavorite> TabFavorites { get; set; }

    public virtual DbSet<TabMenuButton> TabMenuButtons { get; set; }

    public virtual DbSet<TabMenuModule> TabMenuModules { get; set; }

    public virtual DbSet<TabOrder> TabOrders { get; set; }

    public virtual DbSet<TabOrderDetail> TabOrderDetails { get; set; }

    public virtual DbSet<TabRole> TabRoles { get; set; }

    public virtual DbSet<TabRoleAuthority> TabRoleAuthorities { get; set; }

    public virtual DbSet<TabTravel> TabTravels { get; set; }

    public virtual DbSet<TabTravelImg> TabTravelImgs { get; set; }

    public virtual DbSet<TabTravelLeader> TabTravelLeaders { get; set; }

    public virtual DbSet<TabUser> TabUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;uid=root;pwd=wx4510;database=tripweb", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.0.1-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("city")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AreaCode)
                .HasMaxLength(10)
                .HasColumnName("area_code");
            entity.Property(e => e.CenterFlag)
                .HasColumnType("bit(1)")
                .HasColumnName("center_flag");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId).HasColumnName("created_user_id");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.Keyword)
                .HasMaxLength(1000)
                .HasColumnName("keyword");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");
            entity.Property(e => e.UpdatedTime)
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId).HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<County>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("county")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId).HasColumnName("created_user_id");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId).HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("province")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Area)
                .HasMaxLength(10)
                .HasColumnName("area");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId).HasColumnName("created_user_id");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.ShortName)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("short_name");
            entity.Property(e => e.UpdatedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId).HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_category", tb => tb.HasComment("旅游分类"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("主键")
                .HasColumnName("id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasComment("分类名称")
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("最后更新时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabFavorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_favorite", tb => tb.HasComment("收藏夹"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("主键")
                .HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.TravelId)
                .HasComment("旅游产品ID")
                .HasColumnName("travel_id");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("最后更新时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabMenuButton>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_menu_button", tb => tb.HasComment("菜单页面上对应的按钮表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("按扭ID")
                .HasColumnName("id");
            entity.Property(e => e.ButtonName)
                .HasMaxLength(30)
                .HasComment("按钮名称")
                .HasColumnName("button_name");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.IconUrl)
                .HasMaxLength(200)
                .HasComment("菜单图标地址")
                .HasColumnName("icon_url");
            entity.Property(e => e.MenuModuleId)
                .HasComment("所属菜单ID")
                .HasColumnName("menu_module_id");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("最后更新时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabMenuModule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_menu_module", tb => tb.HasComment("菜单模块表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("菜单模块ID")
                .HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.IconUrl)
                .HasMaxLength(200)
                .HasComment("菜单图标地址")
                .HasColumnName("icon_url");
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(200)
                .HasComment("前端页面地址")
                .HasColumnName("link_url");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(30)
                .HasComment("模块名|菜单名")
                .HasColumnName("module_name");
            entity.Property(e => e.ParentMenuModuleId)
                .HasComment("上级模块ID")
                .HasColumnName("parent_menu_module_id");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("最后更新时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_order", tb => tb.HasComment("订单表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.CreatedTime, "idx_created_time");

            entity.HasIndex(e => e.CreatedUserId, "idx_created_user_id");

            entity.HasIndex(e => e.TravelId, "idx_travel_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("订单表ID")
                .HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.TravelId)
                .HasComment("旅游项目ID")
                .HasColumnName("travel_id");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("最后更新时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
            entity.Property(e => e.Version)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasColumnType("timestamp(6)")
                .HasColumnName("version");
        });

        modelBuilder.Entity<TabOrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_order_detail", tb => tb.HasComment("订单人员详情表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.OrderId, "idx_order_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("订单人员详情ID")
                .HasColumnName("id");
            entity.Property(e => e.FriendMobile)
                .HasMaxLength(20)
                .HasComment("亲友电话")
                .HasColumnName("friend_mobile");
            entity.Property(e => e.FriendName)
                .HasMaxLength(30)
                .HasComment("亲友姓名")
                .HasColumnName("friend_name");
            entity.Property(e => e.OrderId)
                .HasComment("所属订单ID")
                .HasColumnName("order_id");
            entity.Property(e => e.OrderUserId)
                .HasComment("下单人ID")
                .HasColumnName("order_user_id");
            entity.Property(e => e.TravelId)
                .HasComment("旅游项目ID")
                .HasColumnName("travel_id");
        });

        modelBuilder.Entity<TabRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_role", tb => tb.HasComment("角色表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("角色ID")
                .HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasComment("角色名称")
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("最后更新时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabRoleAuthority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_role_authority", tb => tb.HasComment("角色权限表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("角色权限ID")
                .HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.MenuButtonId)
                .HasComment("功能按钮ID")
                .HasColumnName("menu_button_id");
            entity.Property(e => e.MenuModuleId)
                .HasComment("所属菜单ID")
                .HasColumnName("menu_module_id");
            entity.Property(e => e.RoleId)
                .HasComment("角色ID")
                .HasColumnName("role_id");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("最后更新时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabTravel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_travel", tb => tb.HasComment("旅游产品信息"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("旅游产品Id")
                .HasColumnName("id");
            entity.Property(e => e.AuditStatus)
                .HasComment("审核状态：1-通过，0-驳回，2-待审核")
                .HasColumnName("audit_status");
            entity.Property(e => e.CategoryId)
                .HasComment("所属分类")
                .HasColumnName("category_id");
            entity.Property(e => e.CityId)
                .HasComment("目的地城市")
                .HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(30)
                .HasComment("目的地城市名称")
                .HasColumnName("city_name");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人|所属商家")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.DetailAddress)
                .HasMaxLength(200)
                .HasComment("目的地具体地址")
                .HasColumnName("detail_address");
            entity.Property(e => e.EndTime)
                .HasComment("结束时间")
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.FavoriteCount)
                .HasDefaultValueSql("'0'")
                .HasComment("收藏次数")
                .HasColumnName("favorite_count");
            entity.Property(e => e.Introduce)
                .HasComment("旅游线路介绍")
                .HasColumnType("text")
                .HasColumnName("introduce");
            entity.Property(e => e.IsLaunched)
                .HasComment("1-已上架,0-未上架")
                .HasColumnName("is_launched");
            entity.Property(e => e.IsThemeTravel)
                .HasComment("是否主题旅游")
                .HasColumnName("is_theme_travel");
            entity.Property(e => e.LaunchedTime)
                .HasComment("上架时间")
                .HasColumnType("datetime")
                .HasColumnName("launched_time");
            entity.Property(e => e.PersonLimit)
                .HasComment("人数上限")
                .HasColumnName("person_limit");
            entity.Property(e => e.Price)
                .HasComment("价格")
                .HasColumnType("double(10,2)")
                .HasColumnName("price");
            entity.Property(e => e.ProvinceId)
                .HasComment("目的地省份")
                .HasColumnName("province_id");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(30)
                .HasComment("目的地省份名称，冗余字段")
                .HasColumnName("province_name");
            entity.Property(e => e.StartTime)
                .HasComment("出发时间")
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.ThumbImgage)
                .HasMaxLength(200)
                .HasComment("缩略图")
                .HasColumnName("thumb_imgage");
            entity.Property(e => e.TravelName)
                .HasMaxLength(500)
                .HasComment("标题")
                .HasColumnName("travel_name");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后更新时间|审核时间|驳回时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
            entity.Property(e => e.Version)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .HasComment("乐观锁")
                .HasColumnType("timestamp(6)")
                .HasColumnName("version");
        });

        modelBuilder.Entity<TabTravelImg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_travel_img", tb => tb.HasComment("旅游产品图片"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BigImg)
                .HasMaxLength(200)
                .HasComment("大图片地址")
                .HasColumnName("big_img");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人|所属商家")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.SmallImg)
                .HasMaxLength(200)
                .HasComment("小图片地址")
                .HasColumnName("small_img");
            entity.Property(e => e.TravelId)
                .HasComment("旅游产品Id")
                .HasColumnName("travel_id");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后更新时间|审核时间|驳回时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabTravelLeader>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_travel_leader", tb => tb.HasComment("景点领队表"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("领队Id")
                .HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.LeaderMobile)
                .HasMaxLength(20)
                .HasComment("领队手机")
                .HasColumnName("leader_mobile");
            entity.Property(e => e.LeaderNickname)
                .HasMaxLength(30)
                .HasComment("领队姓名")
                .HasColumnName("leader_nickname");
            entity.Property(e => e.LeaderUserId)
                .HasComment("领队用户ID")
                .HasColumnName("leader_user_id");
            entity.Property(e => e.TravelId)
                .HasComment("旅游景点ID")
                .HasColumnName("travel_id");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("最后更新时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
        });

        modelBuilder.Entity<TabUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tab_user", tb => tb.HasComment(" 管理员|游客|入驻商家"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => new { e.Email, e.Password }, "idx_email").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 12 });

            entity.HasIndex(e => new { e.Mobile, e.Password }, "idx_mobile").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 12 });

            entity.HasIndex(e => new { e.Username, e.Password }, "idx_username").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 12 });

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("用户ID")
                .HasColumnName("id");
            entity.Property(e => e.ActiveCode)
                .HasComment("激活码")
                .HasColumnName("active_code");
            entity.Property(e => e.ActiveStatus)
                .HasComment("激活状态，1-已激活，0-未激活")
                .HasColumnName("active_status");
            entity.Property(e => e.Birthday)
                .HasComment("生日")
                .HasColumnName("birthday");
            entity.Property(e => e.CreatedTime)
                .HasComment("添加时间")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.CreatedUserId)
                .HasComment("添加人|所属商家")
                .HasColumnName("created_user_id");
            entity.Property(e => e.Deleted)
                .HasComment("1-删除，0-未删除")
                .HasColumnName("deleted");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasComment("邮箱")
                .HasColumnName("email");
            entity.Property(e => e.Mobile)
                .HasMaxLength(30)
                .HasComment("手机号")
                .HasColumnName("mobile");
            entity.Property(e => e.Nickname)
                .HasMaxLength(100)
                .HasComment("真实姓名")
                .HasColumnName("nickname");
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .HasComment("密码")
                .HasColumnName("PASSWORD");
            entity.Property(e => e.RoleId)
                .HasComment("角色ID")
                .HasColumnName("role_id");
            entity.Property(e => e.Sex)
                .HasComment("1-男，2-女,3-未知")
                .HasColumnName("sex");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后更新时间|审核时间|驳回时间")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");
            entity.Property(e => e.UpdatedUserId)
                .HasComment("最后更新人")
                .HasColumnName("updated_user_id");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasComment("用户名")
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
