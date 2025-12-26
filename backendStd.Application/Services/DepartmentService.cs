using backendStd.Application.Dtos;
using backendStd.Application.Dtos.Department;
using backendStd.Application.Dtos.User;
using backendStd.Common.Exceptions;
using backendStd.Core.Entity;
using backendStd.Core.Repository;
using Mapster;

namespace backendStd.Application.Services;

/// <summary>
/// 部门服务
/// </summary>
public class DepartmentService
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IRepository<User> _userRepository;

    public DepartmentService(
        IRepository<Department> departmentRepository,
        IRepository<User> userRepository)
    {
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// 获取部门分页列表
    /// </summary>
    public async Task<PagedResult<DepartmentDto>> GetPagedAsync(PageInput input)
    {
        var (items, total) = await _departmentRepository.GetPagedAsync(input.Page, input.PageSize);
        
        return new PagedResult<DepartmentDto>
        {
            Items = items.Adapt<List<DepartmentDto>>(),
            Total = total,
            Page = input.Page,
            PageSize = input.PageSize
        };
    }

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    public async Task<List<DepartmentDto>> GetTreeAsync()
    {
        var allDepartments = await _departmentRepository.GetListAsync();
        var departmentDtos = allDepartments.Adapt<List<DepartmentDto>>();
        
        return BuildDepartmentTree(departmentDtos, 0);
    }

    /// <summary>
    /// 根据ID获取部门
    /// </summary>
    public async Task<DepartmentDto> GetByIdAsync(long id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            throw new BusinessException("部门不存在");

        return department.Adapt<DepartmentDto>();
    }

    /// <summary>
    /// 新增部门
    /// </summary>
    public async Task<long> AddAsync(DepartmentInput input)
    {
        // 检查部门编码是否存在
        var existDepartments = await _departmentRepository.GetListAsync(d => d.DepartmentCode == input.DepartmentCode);
        if (existDepartments.Any())
            throw new BusinessException("部门编码已存在");

        var department = input.Adapt<Department>();
        
        // 先生成ID（使用雪花ID）
        department.Id = Yitter.IdGenerator.YitIdHelper.NextId();
        
        // 计算部门级别和路径
        if (input.ParentId > 0)
        {
            var parentDepartment = await _departmentRepository.GetByIdAsync(input.ParentId);
            if (parentDepartment == null)
                throw new BusinessException("父级部门不存在");

            department.Level = parentDepartment.Level + 1;
            department.DepartmentPath = $"{parentDepartment.DepartmentPath}/{department.Id}";
        }
        else
        {
            department.Level = 1;
            department.DepartmentPath = $"/{department.Id}";
        }

        await _departmentRepository.InsertAsync(department);
        
        return department.Id;
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    public async Task<bool> UpdateAsync(long id, DepartmentUpdateInput input)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            throw new BusinessException("部门不存在");

        // 检查部门编码是否与其他部门重复
        var existDepartments = await _departmentRepository.GetListAsync(
            d => d.DepartmentCode == input.DepartmentCode && d.Id != id);
        if (existDepartments.Any())
            throw new BusinessException("部门编码已存在");

        // 映射更新字段
        input.Adapt(department);

        return await _departmentRepository.UpdateAsync(department);
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            throw new BusinessException("部门不存在");

        // 检查是否有子部门
        var childDepartments = await _departmentRepository.GetListAsync(d => d.ParentId == id);
        if (childDepartments.Any())
            throw new BusinessException("该部门下存在子部门，无法删除");

        // 检查是否有关联用户
        var users = await _userRepository.GetListAsync(u => u.DepartmentId == id);
        if (users.Any())
            throw new BusinessException("该部门下存在用户，无法删除");

        return await _departmentRepository.SoftDeleteAsync(id);
    }

    /// <summary>
    /// 获取部门下的用户列表
    /// </summary>
    public async Task<List<UserDto>> GetDepartmentUsersAsync(long departmentId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        if (department == null)
            throw new BusinessException("部门不存在");

        var users = await _userRepository.GetListAsync(u => u.DepartmentId == departmentId);
        return users.Adapt<List<UserDto>>();
    }

    /// <summary>
    /// 构建部门树形结构（优化版本，O(n)时间复杂度）
    /// </summary>
    private List<DepartmentDto> BuildDepartmentTree(List<DepartmentDto> allDepartments, long parentId)
    {
        // 使用字典优化查找性能，避免每次都遍历整个列表
        var departmentDict = allDepartments.ToDictionary(d => d.Id);
        var childrenDict = allDepartments
            .GroupBy(d => d.ParentId)
            .ToDictionary(g => g.Key, g => g.OrderBy(d => d.Sort).ToList());

        // 构建树形结构
        void BuildTree(DepartmentDto node)
        {
            if (childrenDict.TryGetValue(node.Id, out var children))
            {
                node.Children = children;
                foreach (var child in children)
                {
                    BuildTree(child);
                }
            }
        }

        // 获取顶级部门
        var rootDepartments = childrenDict.TryGetValue(parentId, out var roots) 
            ? roots 
            : new List<DepartmentDto>();

        // 为每个顶级部门构建子树
        foreach (var root in rootDepartments)
        {
            BuildTree(root);
        }

        return rootDepartments;
    }
}
