using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voting_System.Persistence.EntitiesConfigurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {

        builder.HasIndex(x => new { x.VoteId, x.Content }).IsUnique();

        builder.Property(x => x.Content).HasMaxLength(1500);
    }
}
