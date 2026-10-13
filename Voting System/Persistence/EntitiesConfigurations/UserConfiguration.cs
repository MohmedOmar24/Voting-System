using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voting_System.Abstractions.Consts;

namespace Voting_System.Persistence.EntitiesConfigurations;

public class UserConfiguration:IEntityTypeConfiguration<ApplicatonUser>
{
    public void Configure (EntityTypeBuilder<ApplicatonUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);

        builder.OwnsMany(x => x.RefreshTokens)
               .ToTable("refreshTokens")
               .WithOwner()
               .HasForeignKey("UserId");

        var PasswordHasher=new PasswordHasher<ApplicatonUser>();
        builder.HasData(new ApplicatonUser
        {

            Id = DefaultUsers.AdminId,
            FirstName = "VotingSystem",
            LastName = "Admin",
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            UserName = DefaultUsers.AdminEmail,
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = PasswordHasher.HashPassword(null!, DefaultUsers.AdminPassword)
        });
    }
}
