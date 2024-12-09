using CarAuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionService.Data;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Auction> Auctons { get; set; }
    public DbSet<Item> Items { get; set; }
}

