using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Voting_System.Extensions;
using Voting_System.Persistence.EntitiesConfigurations;

namespace Voting_System.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions
                                 ,IHttpContextAccessor httpContextAccessor)
                    :IdentityDbContext<ApplicatonUser,ApplicationRole,string>(dbContextOptions)
{

    private readonly IHttpContextAccessor _httpContextAccessor=httpContextAccessor;
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<VoteSubmission> VoteSubmissions { get; set; }
    public DbSet<VoteSubmissionAnswer> VoteSubmissionAnswers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var cascadeKes = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);
        
        foreach ( var fk in cascadeKes )
               fk.DeleteBehavior = DeleteBehavior.Restrict;
       
        
        base.OnModelCreating(modelBuilder);
   
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken =default)
    { 
       var entries= ChangeTracker.Entries<AuditableEntity>();
        var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
        foreach(var entityentry in entries)
        {
            if (entityentry.State == EntityState.Added)
                entityentry.Property(x => x.CreatedById).CurrentValue = currentUserId!;
            else if(entityentry.State == EntityState.Modified)
            {
                entityentry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entityentry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }

        }
        return base.SaveChangesAsync(cancellationToken);
    }

}
