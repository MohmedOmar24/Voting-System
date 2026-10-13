using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voting_System.Abstractions.Consts;

namespace Voting_System.Persistence.EntitiesConfigurations;

public class UserRoleConfiguration:IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure (EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        
        builder.HasData(new IdentityUserRole<string>
        {
            UserId=DefaultUsers.AdminId,
            RoleId=DefaultRoles.AdminRoleId
        });
    }
}
