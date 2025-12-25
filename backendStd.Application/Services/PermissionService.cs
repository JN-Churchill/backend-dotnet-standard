using backendStd.Application.Dtos;
using backendStd.Core.Entity;
using backendStd.Core.Repository;
using Mapster;

namespace backendStd.Application.Services;

/// <summary>
/// 权限服务
/// </summary>
public class PermissionService
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;

    public PermissionService(
        IRepository<Permission> permissionRepository,
        IRepository<RolePermission> rolePermissionRepository)
    {
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    /// <summary>
    /// 获取权限列表
    /// </summary>
    public async Task<List<Permission>> GetListAsync()
    {
        var permissions = await _permissionRepository.GetListAsync();
        return permissions.OrderBy(p => p.Sort).ToList();
    }

    /// <summary>
    /// 获取权限树形结构
    /// </summary>
    public async Task<List<Permission>> GetTreeAsync()
    {
        var allPermissions = await GetListAsync();
        return BuildTree(allPermissions, 0);
    }

    /// <summary>
    /// 根据ID获取权限
    /// </summary>
    public async Task<Permission> GetByIdAsync(long id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null)
            throw new Common.Exceptions.BusinessException("权限不存在");

        return permission;
    }

    /// <summary>
    /// 新增权限
    /// </summary>
    public async Task<long> AddAsync(Permission input)
    {
        // 检查权限编码是否存在
        var existPermissions = await _permissionRepository.GetListAsync(p => p.PermissionCode == input.PermissionCode);
        if (existPermissions.Any())
            throw new Common.Exceptions.BusinessException("权限编码已存在");

        return await _permissionRepository.InsertAsync(input);
    }

    /// <summary>
    /// 更新权限
    /// </summary>
    public async Task<bool> UpdateAsync(long id, Permission input)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null)
            throw new Common.Exceptions.BusinessException("权限不存在");

        input.Adapt(permission);
        return await _permissionRepository.UpdateAsync(permission);
    }

    /// <summary>
    /// 删除权限
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        // 检查是否有子权限
        var children = await _permissionRepository.GetListAsync(p => p.ParentId == id);
        if (children.Any())
            throw new Common.Exceptions.BusinessException("存在子权限，无法删除");

        // 检查是否有角色使用该权限
        var rolePermissions = await _rolePermissionRepository.GetListAsync(rp => rp.PermissionId == id);
        if (rolePermissions.Any())
            throw new Common.Exceptions.BusinessException("该权限已被使用，无法删除");

        return await _permissionRepository.SoftDeleteAsync(id);
    }

    /// <summary>
    /// 构建树形结构
    /// </summary>
    private List<Permission> BuildTree(List<Permission> allPermissions, long parentId)
    {
        return allPermissions
            .Where(p => p.ParentId == parentId)
            .Select(p => p)
            .ToList();
    }
}
