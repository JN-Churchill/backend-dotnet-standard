using backendStd.Core.Entity;
using backendStd.Core.Options;
using backendStd.Core.Util;
using Microsoft.Extensions.Options;
using SqlSugar;
using Yitter.IdGenerator;

namespace backendStd.Core.SqlSugarConfig;

/// <summary>
/// 种子数据初始化服务
/// </summary>
public class SeedDataService
{
    private readonly ISqlSugarClient _db;
    private readonly SeedDataOptions _options;

    // 权限编码前缀常量
    private const string PermissionCodePrefix = "permission";
    
    // 默认密码（仅用于开发和测试环境）
    // 生产环境应该禁用种子数据初始化或使用更强的密码
    private const string DefaultPassword = "123456";

    public SeedDataService(ISqlSugarClient db, IOptions<SeedDataOptions> options)
    {
        _db = db;
        _options = options.Value;
    }

    /// <summary>
    /// 初始化种子数据
    /// </summary>
    public async Task InitSeedDataAsync()
    {
        // 获取标记文件的绝对路径（相对于应用程序基目录）
        var flagFilePath = Path.Combine(AppContext.BaseDirectory, _options.SeedDataFlagFile);
        
        // 检查标记文件
        if (File.Exists(flagFilePath))
        {
            Console.WriteLine("[种子数据] 种子数据已初始化，跳过");
            return;
        }

        if (!_options.EnableSeedData)
        {
            Console.WriteLine("[种子数据] 种子数据初始化未启用");
            return;
        }

        Console.WriteLine("[种子数据] 开始初始化种子数据...");

        try
        {
            // 1. 初始化部门数据
            var departments = await InitDepartmentsAsync();
            
            // 2. 初始化角色数据
            var roles = await InitRolesAsync();
            
            // 3. 初始化权限数据
            var permissions = await InitPermissionsAsync();
            
            // 4. 初始化用户数据
            var users = await InitUsersAsync(departments);
            
            // 5. 初始化用户角色关系
            await InitUserRolesAsync(users, roles);
            
            // 6. 初始化角色权限关系
            await InitRolePermissionsAsync(roles, permissions);

            // 创建标记文件
            await File.WriteAllTextAsync(flagFilePath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            Console.WriteLine("[种子数据] 种子数据初始化完成");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[种子数据] 初始化失败: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 初始化部门数据
    /// </summary>
    private async Task<Dictionary<string, long>> InitDepartmentsAsync()
    {
        var departmentIds = new Dictionary<string, long>();

        // 检查是否已有数据
        var count = await _db.Queryable<Department>().CountAsync();
        if (count > 0)
        {
            Console.WriteLine("[种子数据] 部门数据已存在，跳过初始化");
            return departmentIds;
        }

        var now = DateTime.Now;

        // 总公司
        var hqId = YitIdHelper.NextId();
        var hqDept = new Department
        {
            Id = hqId,
            DepartmentName = "总公司",
            DepartmentCode = "HQ",
            ParentId = 0,
            Level = 1,
            DepartmentPath = $"/{hqId}",
            Sort = 1,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        };
        departmentIds["HQ"] = hqId;

        // 技术部
        var techId = YitIdHelper.NextId();
        var techDept = new Department
        {
            Id = techId,
            DepartmentName = "技术部",
            DepartmentCode = "TECH",
            ParentId = hqId,
            Level = 2,
            DepartmentPath = $"/{hqId}/{techId}",
            Sort = 1,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        };
        departmentIds["TECH"] = techId;

        // 运营部
        var opsId = YitIdHelper.NextId();
        var opsDept = new Department
        {
            Id = opsId,
            DepartmentName = "运营部",
            DepartmentCode = "OPS",
            ParentId = hqId,
            Level = 2,
            DepartmentPath = $"/{hqId}/{opsId}",
            Sort = 2,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        };
        departmentIds["OPS"] = opsId;

        await _db.Insertable(new[] { hqDept, techDept, opsDept }).ExecuteCommandAsync();
        Console.WriteLine("[种子数据] 部门数据初始化完成（3条）");

        return departmentIds;
    }

    /// <summary>
    /// 初始化角色数据
    /// </summary>
    private async Task<Dictionary<string, long>> InitRolesAsync()
    {
        var roleIds = new Dictionary<string, long>();

        // 检查是否已有数据
        var count = await _db.Queryable<Role>().CountAsync();
        if (count > 0)
        {
            Console.WriteLine("[种子数据] 角色数据已存在，跳过初始化");
            return roleIds;
        }

        var now = DateTime.Now;

        // 超级管理员
        var superAdminId = YitIdHelper.NextId();
        var superAdminRole = new Role
        {
            Id = superAdminId,
            RoleName = "超级管理员",
            RoleCode = "super_admin",
            Description = "系统超级管理员，拥有所有权限",
            Sort = 1,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        };
        roleIds["super_admin"] = superAdminId;

        // 系统管理员
        var adminId = YitIdHelper.NextId();
        var adminRole = new Role
        {
            Id = adminId,
            RoleName = "系统管理员",
            RoleCode = "admin",
            Description = "系统管理员，拥有除权限管理外的所有权限",
            Sort = 2,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        };
        roleIds["admin"] = adminId;

        // 普通用户
        var userId = YitIdHelper.NextId();
        var userRole = new Role
        {
            Id = userId,
            RoleName = "普通用户",
            RoleCode = "user",
            Description = "普通用户，拥有基本查看权限",
            Sort = 3,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        };
        roleIds["user"] = userId;

        await _db.Insertable(new[] { superAdminRole, adminRole, userRole }).ExecuteCommandAsync();
        Console.WriteLine("[种子数据] 角色数据初始化完成（3条）");

        return roleIds;
    }

    /// <summary>
    /// 初始化权限数据
    /// </summary>
    private async Task<Dictionary<string, long>> InitPermissionsAsync()
    {
        var permissionIds = new Dictionary<string, long>();

        // 检查是否已有数据
        var count = await _db.Queryable<Permission>().CountAsync();
        if (count > 0)
        {
            Console.WriteLine("[种子数据] 权限数据已存在，跳过初始化");
            return permissionIds;
        }

        var now = DateTime.Now;
        var permissions = new List<Permission>();

        // 系统管理（一级菜单）
        var systemId = YitIdHelper.NextId();
        permissions.Add(new Permission
        {
            Id = systemId,
            PermissionName = "系统管理",
            PermissionCode = "system",
            PermissionType = 1,
            ParentId = 0,
            Sort = 1,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        });
        permissionIds["system"] = systemId;

        // 用户管理（二级菜单）
        var userMgmtId = YitIdHelper.NextId();
        permissions.Add(new Permission
        {
            Id = userMgmtId,
            PermissionName = "用户管理",
            PermissionCode = "user_mgmt",
            PermissionType = 1,
            ParentId = systemId,
            Sort = 1,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        });
        permissionIds["user_mgmt"] = userMgmtId;

        // 用户管理操作按钮
        permissions.AddRange(new[]
        {
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "查看用户",
                PermissionCode = "user:view",
                PermissionType = 2,
                ParentId = userMgmtId,
                Sort = 1,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "新增用户",
                PermissionCode = "user:add",
                PermissionType = 2,
                ParentId = userMgmtId,
                Sort = 2,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "编辑用户",
                PermissionCode = "user:edit",
                PermissionType = 2,
                ParentId = userMgmtId,
                Sort = 3,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "删除用户",
                PermissionCode = "user:delete",
                PermissionType = 2,
                ParentId = userMgmtId,
                Sort = 4,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            }
        });

        // 角色管理（二级菜单）
        var roleMgmtId = YitIdHelper.NextId();
        permissions.Add(new Permission
        {
            Id = roleMgmtId,
            PermissionName = "角色管理",
            PermissionCode = "role_mgmt",
            PermissionType = 1,
            ParentId = systemId,
            Sort = 2,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        });
        permissionIds["role_mgmt"] = roleMgmtId;

        // 角色管理操作按钮
        permissions.AddRange(new[]
        {
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "查看角色",
                PermissionCode = "role:view",
                PermissionType = 2,
                ParentId = roleMgmtId,
                Sort = 1,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "新增角色",
                PermissionCode = "role:add",
                PermissionType = 2,
                ParentId = roleMgmtId,
                Sort = 2,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "编辑角色",
                PermissionCode = "role:edit",
                PermissionType = 2,
                ParentId = roleMgmtId,
                Sort = 3,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "删除角色",
                PermissionCode = "role:delete",
                PermissionType = 2,
                ParentId = roleMgmtId,
                Sort = 4,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "分配权限",
                PermissionCode = "role:assign",
                PermissionType = 2,
                ParentId = roleMgmtId,
                Sort = 5,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            }
        });

        // 权限管理（二级菜单）
        var permissionMgmtId = YitIdHelper.NextId();
        permissions.Add(new Permission
        {
            Id = permissionMgmtId,
            PermissionName = "权限管理",
            PermissionCode = "permission_mgmt",
            PermissionType = 1,
            ParentId = systemId,
            Sort = 3,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        });
        permissionIds["permission_mgmt"] = permissionMgmtId;

        // 权限管理操作按钮
        permissions.AddRange(new[]
        {
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "查看权限",
                PermissionCode = "permission:view",
                PermissionType = 2,
                ParentId = permissionMgmtId,
                Sort = 1,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "新增权限",
                PermissionCode = "permission:add",
                PermissionType = 2,
                ParentId = permissionMgmtId,
                Sort = 2,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "编辑权限",
                PermissionCode = "permission:edit",
                PermissionType = 2,
                ParentId = permissionMgmtId,
                Sort = 3,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "删除权限",
                PermissionCode = "permission:delete",
                PermissionType = 2,
                ParentId = permissionMgmtId,
                Sort = 4,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            }
        });

        // 部门管理（二级菜单）
        var deptMgmtId = YitIdHelper.NextId();
        permissions.Add(new Permission
        {
            Id = deptMgmtId,
            PermissionName = "部门管理",
            PermissionCode = "dept_mgmt",
            PermissionType = 1,
            ParentId = systemId,
            Sort = 4,
            Status = 1,
            CreateTime = now,
            IsDeleted = false
        });
        permissionIds["dept_mgmt"] = deptMgmtId;

        // 部门管理操作按钮
        permissions.AddRange(new[]
        {
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "查看部门",
                PermissionCode = "dept:view",
                PermissionType = 2,
                ParentId = deptMgmtId,
                Sort = 1,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "新增部门",
                PermissionCode = "dept:add",
                PermissionType = 2,
                ParentId = deptMgmtId,
                Sort = 2,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "编辑部门",
                PermissionCode = "dept:edit",
                PermissionType = 2,
                ParentId = deptMgmtId,
                Sort = 3,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            },
            new Permission
            {
                Id = YitIdHelper.NextId(),
                PermissionName = "删除部门",
                PermissionCode = "dept:delete",
                PermissionType = 2,
                ParentId = deptMgmtId,
                Sort = 4,
                Status = 1,
                CreateTime = now,
                IsDeleted = false
            }
        });

        // 保存所有权限ID到字典
        foreach (var permission in permissions.Where(p => p.PermissionType == 2))
        {
            permissionIds[permission.PermissionCode] = permission.Id;
        }

        await _db.Insertable(permissions).ExecuteCommandAsync();
        Console.WriteLine($"[种子数据] 权限数据初始化完成（{permissions.Count}条）");

        return permissionIds;
    }

    /// <summary>
    /// 初始化用户数据
    /// </summary>
    private async Task<Dictionary<string, long>> InitUsersAsync(Dictionary<string, long> departments)
    {
        var userIds = new Dictionary<string, long>();

        // 检查是否已有数据
        var count = await _db.Queryable<User>().CountAsync();
        if (count > 0)
        {
            Console.WriteLine("[种子数据] 用户数据已存在，跳过初始化");
            return userIds;
        }

        var now = DateTime.Now;
        var password = MD5Helper.Encrypt(DefaultPassword);

        // 超级管理员
        var superAdminId = YitIdHelper.NextId();
        var superAdmin = new User
        {
            Id = superAdminId,
            UserName = "superadmin",
            Password = password,
            RealName = "超级管理员",
            Email = "superadmin@example.com",
            Phone = "13800138000",
            Status = 1,
            DepartmentId = departments["HQ"],
            CreateTime = now,
            IsDeleted = false
        };
        userIds["superadmin"] = superAdminId;

        // 普通管理员
        var adminId = YitIdHelper.NextId();
        var admin = new User
        {
            Id = adminId,
            UserName = "admin",
            Password = password,
            RealName = "系统管理员",
            Email = "admin@example.com",
            Phone = "13800138001",
            Status = 1,
            DepartmentId = departments["TECH"],
            CreateTime = now,
            IsDeleted = false
        };
        userIds["admin"] = adminId;

        await _db.Insertable(new[] { superAdmin, admin }).ExecuteCommandAsync();
        Console.WriteLine("[种子数据] 用户数据初始化完成（2条）");

        return userIds;
    }

    /// <summary>
    /// 初始化用户角色关系
    /// </summary>
    private async Task InitUserRolesAsync(Dictionary<string, long> users, Dictionary<string, long> roles)
    {
        // 检查是否已有数据
        var count = await _db.Queryable<UserRole>().CountAsync();
        if (count > 0)
        {
            Console.WriteLine("[种子数据] 用户角色关系已存在，跳过初始化");
            return;
        }

        var now = DateTime.Now;
        var userRoles = new List<UserRole>();

        // superadmin -> 超级管理员
        userRoles.Add(new UserRole
        {
            Id = YitIdHelper.NextId(),
            UserId = users["superadmin"],
            RoleId = roles["super_admin"],
            CreateTime = now,
            IsDeleted = false
        });

        // admin -> 系统管理员
        userRoles.Add(new UserRole
        {
            Id = YitIdHelper.NextId(),
            UserId = users["admin"],
            RoleId = roles["admin"],
            CreateTime = now,
            IsDeleted = false
        });

        await _db.Insertable(userRoles).ExecuteCommandAsync();
        Console.WriteLine($"[种子数据] 用户角色关系初始化完成（{userRoles.Count}条）");
    }

    /// <summary>
    /// 初始化角色权限关系
    /// </summary>
    private async Task InitRolePermissionsAsync(Dictionary<string, long> roles, Dictionary<string, long> permissions)
    {
        // 检查是否已有数据
        var count = await _db.Queryable<RolePermission>().CountAsync();
        if (count > 0)
        {
            Console.WriteLine("[种子数据] 角色权限关系已存在，跳过初始化");
            return;
        }

        var now = DateTime.Now;
        var rolePermissions = new List<RolePermission>();

        // 获取所有权限ID
        var allPermissions = await _db.Queryable<Permission>().ToListAsync();
        var allPermissionIds = allPermissions.Select(p => p.Id).ToList();

        // 超级管理员 -> 所有权限
        foreach (var permissionId in allPermissionIds)
        {
            rolePermissions.Add(new RolePermission
            {
                Id = YitIdHelper.NextId(),
                RoleId = roles["super_admin"],
                PermissionId = permissionId,
                CreateTime = now,
                IsDeleted = false
            });
        }

        // 系统管理员 -> 除权限管理外的所有权限
        var adminPermissions = allPermissions
            .Where(p => !p.PermissionCode.StartsWith(PermissionCodePrefix))
            .Select(p => p.Id)
            .ToList();

        foreach (var permissionId in adminPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                Id = YitIdHelper.NextId(),
                RoleId = roles["admin"],
                PermissionId = permissionId,
                CreateTime = now,
                IsDeleted = false
            });
        }

        await _db.Insertable(rolePermissions).ExecuteCommandAsync();
        Console.WriteLine($"[种子数据] 角色权限关系初始化完成（{rolePermissions.Count}条）");
    }
}
