using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voting_System.Persistence.EntitiesConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasIndex(x => x.Title).IsUnique();
        builder.Property(x => x.Title).HasMaxLength(100);
        builder.Property(x=>x.Summary).HasMaxLength(1500);
    }
}
