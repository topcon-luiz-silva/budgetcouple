namespace BudgetCouple.Infrastructure.Persistence;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Accounting.Cartoes;
using BudgetCouple.Domain.Accounting.Categorias;
using BudgetCouple.Domain.Accounting.Contas;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Accounting.Recorrencias;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Common;
using BudgetCouple.Domain.Identity;
using BudgetCouple.Domain.Imports;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

public class AppDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public DbSet<AppConfig> AppConfigs { get; set; } = null!;
    public DbSet<Conta> Contas { get; set; } = null!;
    public DbSet<Cartao> Cartoes { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Lancamento> Lancamentos { get; set; } = null!;
    public DbSet<Recorrencia> Recorrencias { get; set; } = null!;
    public DbSet<Meta> Metas { get; set; } = null!;
    public DbSet<RegraClassificacao> RegrasClassificacao { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Configure Meta TPH
        modelBuilder.Entity<Meta>()
            .HasDiscriminator<string>("Tipo")
            .HasValue<MetaEconomia>("ECONOMIA")
            .HasValue<MetaReducaoCategoria>("REDUCAO_CATEGORIA");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var efTransaction = await Database.BeginTransactionAsync(cancellationToken);
        return new TransactionAdapter(efTransaction);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        // Clear domain events after publishing
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            entry.Entity.ClearDomainEvents();
        }
    }
}

/// <summary>
/// Adapter to wrap EF Core's IDbContextTransaction into our ITransaction interface.
/// </summary>
internal class TransactionAdapter : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public TransactionAdapter(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }
}
