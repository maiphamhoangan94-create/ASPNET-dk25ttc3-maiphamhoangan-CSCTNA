using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;

public partial class AnmphDbContext : DbContext
{
    public AnmphDbContext()
    {
    }

    public AnmphDbContext(DbContextOptions<AnmphDbContext> cauHinh)
        : base(cauHinh)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<PostProvince> PostProvinces { get; set; }
    public DbSet<PostComment> PostComments { get; set; }

    protected override void OnModelCreating(ModelBuilder trinhTaoModel)
    {
        trinhTaoModel.Entity<PostComment>()
            .HasOne(comment => comment.Post)
            .WithMany(post => post.Comments)
            .HasForeignKey(comment => comment.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        trinhTaoModel.Entity<PostComment>()
            .HasOne(comment => comment.User)
            .WithMany(user => user.Comments)
            .HasForeignKey(comment => comment.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        trinhTaoModel.Entity<PostComment>()
            .HasOne(comment => comment.ParentComment)
            .WithMany(comment => comment.Replies)
            .HasForeignKey(comment => comment.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        trinhTaoModel.Entity<PostProvince>()
            .HasKey(postProvince => new { postProvince.PostId, postProvince.ProvinceId });

        trinhTaoModel.Entity<PostProvince>()
            .HasOne(postProvince => postProvince.Post)
            .WithMany(post => post.PostProvinces)
            .HasForeignKey(postProvince => postProvince.PostId);

        trinhTaoModel.Entity<PostProvince>()
            .HasOne(postProvince => postProvince.Province)
            .WithMany(province => province.PostProvinces)
            .HasForeignKey(postProvince => postProvince.ProvinceId);

        trinhTaoModel.Entity<Province>().HasData(
            new Province { Id = 1, Name = "An Giang" },
            new Province { Id = 2, Name = "Bà Rịa - Vũng Tàu" },
            new Province { Id = 3, Name = "Bắc Giang" },
            new Province { Id = 4, Name = "Bắc Kạn" },
            new Province { Id = 5, Name = "Bạc Liêu" },
            new Province { Id = 6, Name = "Bắc Ninh" },
            new Province { Id = 7, Name = "Bến Tre" },
            new Province { Id = 8, Name = "Bình Định" },
            new Province { Id = 9, Name = "Bình Dương" },
            new Province { Id = 10, Name = "Bình Phước" },
            new Province { Id = 11, Name = "Bình Thuận" },
            new Province { Id = 12, Name = "Cà Mau" },
            new Province { Id = 13, Name = "Cao Bằng" },
            new Province { Id = 14, Name = "Đắk Lắk" },
            new Province { Id = 15, Name = "Đắk Nông" },
            new Province { Id = 16, Name = "Điện Biên" },
            new Province { Id = 17, Name = "Đồng Nai" },
            new Province { Id = 18, Name = "Đồng Tháp" },
            new Province { Id = 19, Name = "Gia Lai" },
            new Province { Id = 20, Name = "Hà Giang" },
            new Province { Id = 21, Name = "Hà Nam" },
            new Province { Id = 22, Name = "Hà Tĩnh" },
            new Province { Id = 23, Name = "Hải Dương" },
            new Province { Id = 24, Name = "Hậu Giang" },
            new Province { Id = 25, Name = "Hòa Bình" },
            new Province { Id = 26, Name = "Hưng Yên" },
            new Province { Id = 27, Name = "Khánh Hòa" },
            new Province { Id = 28, Name = "Kiên Giang" },
            new Province { Id = 29, Name = "Kon Tum" },
            new Province { Id = 30, Name = "Lai Châu" },
            new Province { Id = 31, Name = "Lâm Đồng" },
            new Province { Id = 32, Name = "Lạng Sơn" },
            new Province { Id = 33, Name = "Lào Cai" },
            new Province { Id = 34, Name = "Long An" },
            new Province { Id = 35, Name = "Nam Định" },
            new Province { Id = 36, Name = "Nghệ An" },
            new Province { Id = 37, Name = "Ninh Bình" },
            new Province { Id = 38, Name = "Ninh Thuận" },
            new Province { Id = 39, Name = "Phú Thọ" },
            new Province { Id = 40, Name = "Phú Yên" },
            new Province { Id = 41, Name = "Quảng Bình" },
            new Province { Id = 42, Name = "Quảng Nam" },
            new Province { Id = 43, Name = "Quảng Ngãi" },
            new Province { Id = 44, Name = "Quảng Ninh" },
            new Province { Id = 45, Name = "Quảng Trị" },
            new Province { Id = 46, Name = "Sóc Trăng" },
            new Province { Id = 47, Name = "Sơn La" },
            new Province { Id = 48, Name = "Tây Ninh" },
            new Province { Id = 49, Name = "Thái Bình" },
            new Province { Id = 50, Name = "Thái Nguyên" },
            new Province { Id = 51, Name = "Thanh Hóa" },
            new Province { Id = 52, Name = "Thừa Thiên Huế" },
            new Province { Id = 53, Name = "Tiền Giang" },
            new Province { Id = 54, Name = "Trà Vinh" },
            new Province { Id = 55, Name = "Tuyên Quang" },
            new Province { Id = 56, Name = "Vĩnh Long" },
            new Province { Id = 57, Name = "Vĩnh Phúc" },
            new Province { Id = 58, Name = "Yên Bái" },
            new Province { Id = 59, Name = "Cần Thơ" },
            new Province { Id = 60, Name = "Đà Nẵng" },
            new Province { Id = 61, Name = "Hải Phòng" },
            new Province { Id = 62, Name = "Hà Nội" },
            new Province { Id = 63, Name = "TP Hồ Chí Minh" });

        OnModelCreatingPartial(trinhTaoModel);
    }

    partial void OnModelCreatingPartial(ModelBuilder trinhTaoModel);
}
