using Microsoft.EntityFrameworkCore;
using NegotiationSystemAPI.Models;

namespace NegotiationSystemAPI.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Party> Parties { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<PaymentRatio> PaymentRatios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentRatio>()
        .HasOne(pr => pr.Proposal)
        .WithMany(p => p.PaymentRatios)
        .HasForeignKey(pr => pr.ProposalId)
        .OnDelete(DeleteBehavior.NoAction);
    }
}
