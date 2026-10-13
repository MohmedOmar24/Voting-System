using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voting_System.Persistence.EntitiesConfigurations;

public class VoteSubmissionAnswerConfiguration : IEntityTypeConfiguration<VoteSubmissionAnswer>
{
    public void Configure(EntityTypeBuilder<VoteSubmissionAnswer> builder)
    {
        builder.HasIndex(x => new { x.VoteSubmissionId, x.QuestionId}).IsUnique();

    }
}