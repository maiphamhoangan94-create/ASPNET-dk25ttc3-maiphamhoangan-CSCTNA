using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;

public partial class AnmphDbContext : DbContext
{
    public AnmphDbContext()
    {
    }

    public AnmphDbContext(DbContextOptions<AnmphDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
