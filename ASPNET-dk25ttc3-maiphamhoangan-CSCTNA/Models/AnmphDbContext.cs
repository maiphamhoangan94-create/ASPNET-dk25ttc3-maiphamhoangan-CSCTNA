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

    protected override void OnModelCreating(ModelBuilder trinhTaoModel)
    {
        OnModelCreatingPartial(trinhTaoModel);
    }

    partial void OnModelCreatingPartial(ModelBuilder trinhTaoModel);
}
