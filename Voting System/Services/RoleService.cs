using Microsoft.AspNetCore.Identity;
using Voting_System.Contracts.Roles;
using Mapster;
using Voting_System.Errors;
using Voting_System.Persistence;
using Voting_System.Abstractions.Consts;

namespace Voting_System.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager,
                         ApplicationDbContext context            ):IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManger = roleManager;
    private readonly ApplicationDbContext _context=context;
    
    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken=default)
        =>await  _roleManger.Roles.Where(r=>!r.IsDefault &&(!r.IsDeleted||(includeDisabled.HasValue&& includeDisabled.Value)))
        .ProjectToType<RoleResponse>()
        .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailsResponse>> GetAsync(string id, CancellationToken cancellationToken=default)
    {
        if (await _roleManger.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);
        var permissions=await _roleManger.GetClaimsAsync(role);
        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted,permissions.Select(c=>c.Value));
        return Result.Success(response);
    }

    public async Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request , CancellationToken cancellationToken = default)
    {
        var isExist = await _roleManger.RoleExistsAsync(request.Name);
        if (isExist)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

        var allowedPermissions = Permissions.GetAllPermissions();
        if(request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermission);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        var result= await _roleManger.CreateAsync(role);
        if(result.Succeeded)
        {
            var permissions = request.Permissions.Select(
                x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id,
                });
           await _context.AddRangeAsync(permissions,cancellationToken);     
           await _context.SaveChangesAsync(cancellationToken);

            var response = new RoleDetailsResponse(role.Id, role.Name, role.IsDeleted, request.Permissions);
            return Result.Success(response);
        }

        var error = result.Errors.First();
        return Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
