using FigCommissionAnalyticsEngine.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FigCommissionAnalyticsEngine.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Agent> Agents { get; set; }
    public DbSet<Carrier> Carriers { get; set; }
    public DbSet<AgentCarrier> AgentCarriers { get; set; }
    public DbSet<AgentCarrierCommissionStatement> AgentCarrierCommissionStatements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Agent configuration
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.ToTable("Agent");
            entity.HasKey(e => e.AgentId);
            entity.Property(e => e.AgentId).HasColumnName("AgentId");
            entity.Property(e => e.AgentName).HasColumnName("AgentName").IsRequired();
        });

        // Carrier configuration
        modelBuilder.Entity<Carrier>(entity =>
        {
            entity.ToTable("Carrier");
            entity.HasKey(e => e.CarrierId);
            entity.Property(e => e.CarrierId).HasColumnName("CarrierId");
            entity.Property(e => e.CarrierName).HasColumnName("CarrierName").IsRequired();
        });

        // AgentCarrier configuration
        modelBuilder.Entity<AgentCarrier>(entity =>
        {
            entity.ToTable("AgentCarrier");
            entity.HasKey(e => e.AgentCarrierId);
            entity.Property(e => e.AgentCarrierId).HasColumnName("AgentCarrierId");
            entity.Property(e => e.AgentId).HasColumnName("AgentId");
            entity.Property(e => e.CarrierId).HasColumnName("CarrierId");
            entity.Property(e => e.WritingNumber).HasColumnName("WritingNumber").IsRequired();

            entity.HasOne(e => e.Agent)
                .WithMany(a => a.AgentCarriers)
                .HasForeignKey(e => e.AgentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Carrier)
                .WithMany(c => c.AgentCarriers)
                .HasForeignKey(e => e.CarrierId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AgentCarrierCommissionStatement configuration
        modelBuilder.Entity<AgentCarrierCommissionStatement>(entity =>
        {
            entity.ToTable("AgentCarrierCommissionStatement");
            entity.HasKey(e => e.AgentCarrierCommissionStatementId);
            entity.Property(e => e.AgentCarrierCommissionStatementId).HasColumnName("AgentCarrierCommissionStatementId");
            entity.Property(e => e.AgentCarrierId).HasColumnName("AgentCarrierId");
            entity.Property(e => e.CommissionAmount).HasColumnName("CommissionAmount");
            entity.Property(e => e.StatementDate).HasColumnName("StatementDate").IsRequired();

            entity.HasOne(e => e.AgentCarrier)
                .WithMany(ac => ac.CommissionStatements)
                .HasForeignKey(e => e.AgentCarrierId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
