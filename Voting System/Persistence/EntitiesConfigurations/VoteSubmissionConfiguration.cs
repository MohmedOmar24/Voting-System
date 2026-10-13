using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voting_System.Persistence.EntitiesConfigurations;

public class VoteSubmissionConfiguration : IEntityTypeConfiguration<VoteSubmission>
{
    public void Configure(EntityTypeBuilder<VoteSubmission> builder)
    {
        builder.HasIndex(x => new { x.VoteId, x.UserId }).IsUnique();

    }
}