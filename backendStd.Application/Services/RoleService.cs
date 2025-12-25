using backendStd.Application.Dtos;
using backendStd.Core.Entity;
using backendStd.Core.Repository;
using Mapster;

namespace backendStd.Application.Services;

/// <summary>
/// 角色服务
/// </summary>
public class RoleService
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<UserRole> _userRoleRepository;

    public RoleService(
        IRepository<Role> roleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<UserRole> userRoleRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    /// <summary>
    /// 获取角色分页列表
    /// </summary>
    public async Task<PagedResult<Role>> GetPagedAsync(PageInput input)
    {
        var (items, total) = await _roleRepository.GetPagedAsync(input.Page, input.PageSize);
        
        return new PagedResult<Role>
        {
            Items = items,
            Total = total,
            Page = input.Page,
            PageSize = input.PageSize
        };
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    public async Task<Role> GetByIdAsync(long id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new Common.Exceptions.BusinessException("角色不存在");

        return role;
    }

    /// <summary>
    /// 新增角色
    /// </summary>
    public async Task<long> AddAsync(Role input)
    {
        // 检查角色编码是否存在
        var existRoles = await _roleRepository.GetListAsync(r => r.RoleCode == input.RoleCode);
        if (existRoles.Any())
            throw new Common.Exceptions.BusinessException("角色编码已存在");

        return await _roleRepository.InsertAsync(input);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    public async Task<bool> UpdateAsync(long id, Role input)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new Common.Exceptions.BusinessException("角色不存在");

        input.Adapt(role);
        return await _roleRepository.UpdateAsync(role);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        // 检查是否有用户使用该角色
        var userRoles = await _userRoleRepository.GetListAsync(ur => ur.RoleId == id);
        if (userRoles.Any())
            throw new Common.Exceptions.BusinessException("该角色已被使用，无法删除");

        return await _roleRepository.SoftDeleteAsync(id);
    }

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    public async Task<bool> AssignPermissionsAsync(long roleId, List<long> permissionIds)
    {
        // 删除旧的权限关系
        var oldRolePermissions = await _rolePermissionRepository.GetListAsync(rp => rp.RoleId == roleId);
        if (oldRolePermissions.Any())
        {
            // 使用批量软删除
            var ids = oldRolePermissions.Select(rp => rp.Id).ToList();
            await _rolePermissionRepository.SoftDeleteAsync(ids);
        }

        // 添加新的权限关系
        var rolePermissions = permissionIds.Select(permissionId => new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId
        }).ToList();

        await _rolePermissionRepository.BatchInsertAsync(rolePermissions);
        return true;
    }

    /// <summary>
    /// 获取角色的权限列表
    /// </summary>
    public async Task<List<long>> GetRolePermissionsAsync(long roleId)
    {
        var rolePermissions = await _rolePermissionRepository.GetListAsync(rp => rp.RoleId == roleId);
        return rolePermissions.Select(rp => rp.PermissionId).ToList();
    }
}
