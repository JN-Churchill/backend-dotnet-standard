# Backend Standard - .NET 8 企业级后端标准模板

> **最后更新**: 2025年12月25日  
> **适用场景**: 企业级Web API开发、微服务后端、SaaS平台  
> **技术栈**: .NET 8 + SqlSugar + JWT + Redis + Quartz

---

## 📖 目录

1. [项目概述](#项目概述)
2. [核心特性](#核心特性)
3. [技术栈](#技术栈)
4. [项目结构](#项目结构)
5. [核心功能模块](#核心功能模块)
6. [快速开始](#快速开始)
7. [开发指南](#开发指南)
8. [API接口规范](#api接口规范)
9. [最佳实践](#最佳实践)
10. [部署指南](#部署指南)

---

## 项目概述

这是一个基于 **.NET 8** 构建的**企业级后端标准模板项目**，遵循 **DDD（领域驱动设计）** 分层架构，集成了企业级项目常用的基础设施和功能模块，开箱即用。

### 🎯 设计目标

- ✅ **纯标准.NET实现** - 无第三方框架依赖，使用ASP.NET Core原生API
- ✅ **完整的分层架构** - 严格的DDD四层架构，职责清晰
- ✅ **生产就绪** - 包含企业级项目所需的所有基础设施
- ✅ **易于扩展** - 模块化设计，方便根据业务需求定制

---

## 核心特性

### ✅ 已实现功能

#### 基础架构
- [x] 4层架构设计（Entry/Application/Core/Common）
- [x] SqlSugar ORM集成，支持多数据库（MySQL/SqlServer/PostgreSQL）
- [x] 雪花ID主键生成策略
- [x] 实体基类（审计字段、软删除）
- [x] 仓储模式封装（CRUD、分页）
- [x] 统一返回结果格式
- [x] 种子数据初始化（自动创建默认用户、角色、权限、部门）

#### 认证授权
- [x] JWT认证完整实现（AccessToken + RefreshToken）
- [x] 权限管理（RBAC角色权限系统）
- [x] 数据权限过滤（全部/本人/本部门等）
- [x] 权限验证特性（RequirePermissionAttribute）

#### 缓存与日志
- [x] 内存缓存 & Redis缓存
- [x] Serilog日志记录（Console + File）
- [x] 请求日志中间件（记录API调用详情）

#### 数据验证与异常处理
- [x] FluentValidation数据验证
- [x] 全局异常过滤器
- [x] 自定义业务异常和验证异常

#### 其他企业级功能
- [x] Mapster对象映射
- [x] 接口限流中间件（防止暴力攻击）
- [x] Quartz.NET定时任务框架
- [x] 文件上传服务
- [x] Swagger API文档自动生成

#### 示例模块
- [x] 用户管理（登录、CRUD、刷新Token）
- [x] 角色管理（CRUD、权限分配）
- [x] 权限管理（CRUD、树形结构）
- [x] 部门管理（CRUD、树形结构、用户列表）
- [x] Demo示例模块（完整CRUD）
- [x] 定时任务管理（暂停、恢复、触发）
- [x] 操作日志记录

---

## 技术栈

### 核心技术

| 技术 | 版本 | 用途 | 说明 |
|------|------|------|------|
| **.NET** | 8.0 | 基础框架 | 最新LTS版本，性能优越 |
| **ASP.NET Core** | 8.0 | Web框架 | RESTful API开发 |
| **SqlSugar** | 5.1.4.162 | ORM | 高性能、多数据库支持 |
| **Serilog** | 8.0.3 | 日志框架 | 结构化日志，支持多sink |
| **FluentValidation** | 11.10.0 | 数据验证 | 流式验证，规则清晰 |
| **Mapster** | 7.4.0 | 对象映射 | 高性能DTO映射 |
| **Quartz.NET** | 3.13.1 | 任务调度 | 定时任务框架 |
| **JWT** | 8.0.2 | 认证 | Token认证机制 |
| **StackExchange.Redis** | 2.8.16 | 缓存 | Redis客户端 |
| **Yitter.IdGenerator** | 1.0.14 | ID生成 | 雪花算法 |
| **Swashbuckle** | - | API文档 | Swagger自动生成 |

### 数据库支持

✅ **MySQL** (默认，已配置)  
✅ **SQL Server** (SqlSugar支持)  
✅ **PostgreSQL** (SqlSugar支持)  
✅ **SQLite** (SqlSugar支持)  
✅ **Oracle** (SqlSugar支持)

---

## 项目结构

### 分层架构

```
┌─────────────────────────────────────────────────┐
│  backendStd.Web.Entry  [表现层/启动层]          │
│  - 应用程序入口                                  │
│  - 中间件管道配置                                │
│  - 依赖注入配置                                  │
│  - 认证授权配置                                  │
└─────────────────────────────────────────────────┘
                      ↓ 依赖
┌─────────────────────────────────────────────────┐
│  backendStd.Application  [应用层]                │
│  - Controllers (API接口)                         │
│  - DTOs (数据传输对象)                           │
│  - Services (业务服务)                           │
│  - Validators (数据验证器)                       │
└─────────────────────────────────────────────────┘
                      ↓ 依赖
┌─────────────────────────────────────────────────┐
│  backendStd.Core  [领域层/核心层]                │
│  - Entity (实体模型)                             │
│  - Repository (仓储接口/实现)                    │
│  - Auth (认证授权)                               │
│  - Cache (缓存服务)                              │
│  - Jobs (定时任务)                               │
│  - Filters (过滤器)                              │
│  - Middleware (中间件)                           │
│  - Options (配置选项)                            │
│  - Enum/Const (枚举/常量)                        │
│  - Util (工具类)                                 │
└─────────────────────────────────────────────────┘
                      ↓ 依赖
┌─────────────────────────────────────────────────┐
│  backendStd.Common  [基础设施层]                 │
│  - Exceptions (自定义异常)                       │
│  - Models (通用模型)                             │
└─────────────────────────────────────────────────┘
```

### 详细目录结构

```
backendStd.sln
├── backendStd.Web.Entry          [启动层]
│   ├── Program.cs                 启动配置
│   ├── appsettings.json          配置文件
│   └── wwwroot/                  静态文件目录
│
├── backendStd.Application        [应用层]
│   ├── Controllers/              控制器
│   │   ├── UserController.cs     用户管理（8个API）
│   │   ├── RoleController.cs     角色管理（7个API）
│   │   ├── PermissionController.cs 权限管理（6个API）
│   │   ├── DepartmentController.cs 部门管理（7个API）
│   │   ├── JobController.cs      任务管理（5个API）
│   │   └── DemoController.cs     示例控制器（6个API）
│   ├── Dtos/                     数据传输对象
│   │   ├── PageInput.cs          分页输入
│   │   ├── PagedResult.cs        分页结果
│   │   ├── User/                 用户DTO
│   │   ├── Role/                 角色DTO
│   │   ├── Permission/           权限DTO
│   │   ├── Department/           部门DTO
│   │   └── Demo/                 演示DTO
│   ├── Services/                 业务服务
│   │   ├── UserService.cs        用户服务
│   │   ├── RoleService.cs        角色服务
│   │   ├── PermissionService.cs  权限服务
│   │   ├── DepartmentService.cs  部门服务
│   │   ├── JobService.cs         任务服务
│   │   ├── DemoService.cs        演示服务
│   │   └── FileService.cs        文件服务
│   └── Validators/               验证器
│       ├── UserValidator.cs      用户验证
│       └── DemoValidator.cs      演示验证
│
├── backendStd.Core               [核心层]
│   ├── Entity/                   实体模型
│   │   ├── Base/
│   │   │   └── EntityBase.cs     实体基类(雪花ID、审计字段)
│   │   ├── User.cs               用户实体
│   │   ├── Role.cs               角色实体
│   │   ├── Permission.cs         权限实体
│   │   ├── RolePermission.cs     角色权限关系
│   │   ├── UserRole.cs           用户角色关系
│   │   ├── Department.cs         部门实体
│   │   ├── OperationLog.cs       操作日志实体
│   │   └── Demo.cs               演示实体
│   ├── Repository/               仓储模式
│   │   ├── IRepository.cs        仓储接口
│   │   └── SqlSugarRepository.cs SqlSugar实现
│   ├── SqlSugarConfig/           数据库配置
│   │   ├── SqlSugarSetup.cs      SqlSugar初始化
│   │   └── SeedDataService.cs    种子数据服务
│   ├── Auth/                     认证授权
│   │   ├── JwtHandler.cs         JWT处理器
│   │   ├── RequirePermissionAttribute.cs 权限验证特性
│   │   ├── RateLimitAttribute.cs 限流特性
│   │   └── DataPermissionAttribute.cs 数据权限特性
│   ├── Cache/                    缓存服务
│   │   ├── ICacheService.cs      缓存接口
│   │   ├── MemoryCacheService.cs 内存缓存
│   │   └── RedisCacheService.cs  Redis缓存
│   ├── Jobs/                     定时任务
│   │   ├── BaseJob.cs            任务基类
│   │   ├── DataCleanupJob.cs     数据清理任务
│   │   └── DataStatisticsJob.cs  数据统计任务
│   ├── Filters/                  过滤器
│   │   ├── GlobalExceptionFilter.cs 全局异常过滤
│   │   └── DataPermissionFilter.cs  数据权限过滤
│   ├── Middleware/               中间件
│   │   ├── RequestLoggingMiddleware.cs 请求日志
│   │   └── RateLimitingMiddleware.cs   接口限流
│   ├── Options/                  配置选项
│   │   ├── DbConnectionOptions.cs 数据库配置
│   │   ├── JWTSettingsOptions.cs JWT配置
│   │   ├── RedisOptions.cs       Redis配置
│   │   ├── SeedDataOptions.cs    种子数据配置
│   │   └── FileUploadOptions.cs  文件上传配置
│   ├── Enum/                     枚举定义
│   ├── Const/                    常量定义
│   ├── Util/                     工具类
│   │   ├── TdivsResultProvider.cs 统一返回格式
│   │   ├── MD5Helper.cs          MD5加密
│   │   └── JsonHelper.cs         JSON工具
│   └── Extension/                扩展方法
│
└── backendStd.Common             [通用层]
    └── Exceptions/
        ├── BusinessException.cs   业务异常
        └── ValidationException.cs 验证异常
```

---

## 核心功能模块

### 1. 用户认证与授权

#### JWT认证机制
- **AccessToken**: 有效期2小时，用于API调用
- **RefreshToken**: 有效期30天，用于刷新AccessToken
- **Claims载荷**: 用户ID、用户名、真实姓名、手机号、邮箱等

#### 权限控制
```csharp
// 方法级权限控制
[RequirePermission("user:view")]
public async Task<PagedResult<UserDto>> GetPageAsync([FromQuery] PageInput input)
{
    // 只有拥有 user:view 权限的用户才能访问
}

// 数据权限过滤
[DataPermission(nameof(User))]
public async Task<List<UserDto>> GetListAsync()
{
    // 根据用户的数据权限范围自动过滤数据
}
```

#### 接口限流
```csharp
[RateLimit(MaxRequests = 10, TimeWindowInSeconds = 60)]
public async Task<IActionResult> LoginAsync([FromBody] LoginInput input)
{
    // 限制每分钟最多10次请求
}
```

---

### 2. RBAC权限管理

#### 权限模型
```
用户(User) ──┬── 用户角色(UserRole) ──> 角色(Role)
            │                            │
            └── 部门(Department)          ├── 角色权限(RolePermission)
                                         │
                                         └──> 权限(Permission)
```

#### 权限编码规范
- **模块:操作** 格式，如 `user:view`, `user:add`, `user:edit`, `user:delete`
- **通配符支持**: `user:*` 表示用户模块所有权限

#### 数据权限级别
```csharp
public enum DataScopeEnum
{
    All = 1,            // 全部数据
    Self = 2,           // 仅本人数据
    SelfAndChildren = 3 // 本人及下属数据
}
```

---

### 3. 数据审计

#### 审计字段（EntityBase）
```csharp
public abstract class EntityBase
{
    public long Id { get; set; }              // 雪花ID
    public DateTime CreateTime { get; set; }   // 创建时间
    public DateTime? UpdateTime { get; set; }  // 更新时间
    public long? CreateUserId { get; set; }    // 创建人ID
    public long? UpdateUserId { get; set; }    // 更新人ID
    public bool IsDeleted { get; set; }        // 软删除标记
    public DateTime? DeleteTime { get; set; }  // 删除时间
}
```

#### 自动填充审计字段
SqlSugar AOP会在插入和更新时自动填充审计字段，确保数据变更可追溯。

---

### 4. 缓存服务

#### 统一缓存接口
```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}
```

#### 双实现切换
- **MemoryCacheService**: 内存缓存（单机）
- **RedisCacheService**: Redis缓存（分布式）

---

### 5. 定时任务

#### Quartz.NET集成
```csharp
builder.Services.AddQuartz(q =>
{
    // 数据清理任务 - 每天凌晨2点
    var jobKey = new JobKey("DataCleanupJob");
    q.AddJob<DataCleanupJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithCronSchedule("0 0 2 * * ?"));
});
```

#### 内置任务
- **DataCleanupJob**: 清理过期数据（软删除数据物理删除、过期日志清理）
- **DataStatisticsJob**: 数据统计（用户活跃度、业务指标统计）

#### 任务管理API
```
POST   /api/job/trigger/{jobName}    // 手动触发任务
GET    /api/job/list                  // 获取任务列表
POST   /api/job/pause/{jobName}       // 暂停任务
POST   /api/job/resume/{jobName}      // 恢复任务
DELETE /api/job/{jobName}             // 删除任务
```

---

### 6. 文件上传

#### 功能特性
- ✅ 文件大小验证（默认10MB）
- ✅ 文件类型白名单（图片、文档、压缩包等）
- ✅ 按日期分目录存储 `wwwroot/uploads/2025/12/25/`
- ✅ 雪花ID重命名（防止文件名冲突）
- ✅ 返回访问URL路径

---

### 7. 统一异常处理

#### 全局异常过滤器
自动捕获所有未处理的异常，区分业务异常、验证异常、系统异常，返回统一格式的错误信息。

#### 自定义异常
```csharp
// 业务异常
throw new BusinessException("用户不存在");

// 验证异常
throw new ValidationException("手机号格式不正确");
```

---

### 8. 数据验证

#### FluentValidation验证器
```csharp
public class UserInputValidator : AbstractValidator<UserInput>
{
    public UserInputValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空")
            .Length(2, 20).WithMessage("用户名长度必须在2-20个字符之间");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于6位");
    }
}
```

验证失败会自动返回400状态码和详细的错误信息。

---

### 9. 请求日志

#### 中间件自动记录
RequestLoggingMiddleware会自动记录所有HTTP请求和响应：
- 请求路径、方法、参数
- 响应状态码、耗时
- 用户信息（如果已认证）
- 客户端IP地址

---

## 快速开始

### 1. 环境要求
- .NET 8 SDK
- MySQL 5.7+ / SQL Server 2016+ / PostgreSQL 9.6+
- Visual Studio 2022 / Rider / VS Code
- Redis（可选，使用Redis缓存时需要）

### 2. 配置数据库

编辑 `backendStd.Web.Entry/appsettings.json`:

```json
{
  "DbConnectionOptions": {
    "ConnectionConfigs": [
      {
        "ConfigId": "Main",
        "DbType": "MySql",
        "ConnectionString": "server=localhost;Database=backendstd;Uid=root;Pwd=123456;",
        "IsAutoCloseConnection": true,
        "EnableInitDb": true,
        "EnableSqlLog": true
      }
    ]
  }
}
```

### 3. 种子数据配置（可选）

种子数据功能会在首次启动时自动创建默认数据。配置说明：

```json
{
  "SeedDataOptions": {
    "EnableSeedData": true,  // 是否启用种子数据初始化
    "SeedDataFlagFile": "seed_data_initialized.flag"  // 标记文件路径
  }
}
```

#### 默认种子数据包括：

**部门数据**：
- 总公司 (HQ)
- 技术部 (TECH)
- 运营部 (OPS)

**角色数据**：
- 超级管理员 (super_admin)
- 系统管理员 (admin)
- 普通用户 (user)

**权限数据**（树形结构）：
- 系统管理 → 用户管理 → 查看/新增/编辑/删除
- 系统管理 → 角色管理 → 查看/新增/编辑/删除/分配权限
- 系统管理 → 权限管理 → 查看/新增/编辑/删除
- 系统管理 → 部门管理 → 查看/新增/编辑/删除

**默认用户**：
- 用户名: `superadmin`, 密码: `123456` (超级管理员，拥有所有权限)
- 用户名: `admin`, 密码: `123456` (系统管理员，拥有除权限管理外的所有权限)

> **注意**: 种子数据仅在首次启动且数据库为空时初始化。如需重新初始化，请删除 `seed_data_initialized.flag` 文件并清空数据库。

### 4. 配置JWT密钥

```json
{
  "JWTSettings": {
    "IssuerSigningKey": "your-256-bit-secret-key-change-this-in-production",
    "ValidIssuer": "backendStd",
    "ValidAudience": "backendStd.Client",
    "ExpiredTime": 120,
    "ValidateIssuerSigningKey": true,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ClockSkew": 5
  }
}
```

### 5. 运行项目

```bash
cd backendStd.Web.Entry
dotnet run
```

### 6. 访问Swagger

浏览器打开: `http://localhost:5000/swagger`

### 7. 测试登录API

```bash
curl -X POST http://localhost:5000/api/user/login \
  -H "Content-Type: application/json" \
  -d '{"UserName":"admin","Password":"123456"}'
```

---

## 开发指南

### 开发新功能流程

#### Step 1: 创建实体 (Core/Entity)
```csharp
[SugarTable("sys_article")]
public class Article : EntityBase
{
    [SugarColumn(ColumnDescription = "标题", Length = 200)]
    public string Title { get; set; }
    
    [SugarColumn(ColumnDescription = "内容", ColumnDataType = "text")]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDescription = "作者ID")]
    public long AuthorId { get; set; }
}
```

#### Step 2: 创建DTOs (Application/Dtos)
```csharp
public class ArticleDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreateTime { get; set; }
}

public class ArticleInput
{
    public string Title { get; set; }
    public string Content { get; set; }
}
```

#### Step 3: 创建验证器 (Application/Validators)
```csharp
public class ArticleInputValidator : AbstractValidator<ArticleInput>
{
    public ArticleInputValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("标题不能为空")
            .MaximumLength(200).WithMessage("标题不能超过200个字符");
    }
}
```

#### Step 4: 创建服务 (Application/Services)
```csharp
public class ArticleService
{
    private readonly IRepository<Article> _repository;
    
    public async Task<PagedResult<ArticleDto>> GetPageAsync(PageInput input)
    {
        var query = _repository.AsQueryable()
            .Where(x => !x.IsDeleted);
            
        var total = await query.CountAsync();
        var items = await query
            .Skip((input.PageIndex - 1) * input.PageSize)
            .Take(input.PageSize)
            .ToListAsync();
            
        return new PagedResult<ArticleDto>
        {
            Items = items.Adapt<List<ArticleDto>>(),
            Total = total,
            PageIndex = input.PageIndex,
            PageSize = input.PageSize
        };
    }
}
```

#### Step 5: 创建控制器 (Application/Controllers)
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArticleController : ControllerBase
{
    private readonly ArticleService _articleService;
    
    [HttpGet("page")]
    [RequirePermission("article:view")]
    public async Task<PagedResult<ArticleDto>> GetPageAsync([FromQuery] PageInput input)
    {
        return await _articleService.GetPageAsync(input);
    }
}
```

#### Step 6: 注册服务 (Program.cs)
```csharp
builder.Services.AddScoped<ArticleService>();
```

#### Step 7: 运行项目
首次运行时SqlSugar会自动创建表结构。

---

## API接口规范

### RESTful风格

| 方法 | 路径 | 说明 | 示例 |
|------|------|------|------|
| GET | /api/user/page | 分页查询 | ?pageIndex=1&pageSize=10 |
| GET | /api/user/{id} | 获取单个 | /api/user/123456789 |
| POST | /api/user | 新增 | Body: {...} |
| PUT | /api/user/{id} | 更新 | /api/user/123456789 |
| DELETE | /api/user/{id} | 删除 | /api/user/123456789 |
| DELETE | /api/user/batch | 批量删除 | Body: [id1, id2] |

### 统一返回格式

#### 成功响应
```json
{
  "Code": 200,
  "Type": "success",
  "Message": null,
  "Result": {
    "id": 123456789,
    "userName": "admin"
  },
  "Extras": null,
  "Time": "2025-12-25T10:30:00"
}
```

#### 分页响应
```json
{
  "Code": 200,
  "Type": "success",
  "Message": null,
  "Result": {
    "items": [{...}, {...}],
    "total": 100,
    "pageIndex": 1,
    "pageSize": 10
  },
  "Extras": null,
  "Time": "2025-12-25T10:30:00"
}
```

#### 失败响应
```json
{
  "Code": 400,
  "Type": "error",
  "Message": "用户名或密码错误",
  "Result": null,
  "Extras": null,
  "Time": "2025-12-25T10:30:00"
}
```

### API端点统计

- **用户管理**: 8个API（登录、刷新Token、分页查询、详情、新增、更新、删除、批量删除）
- **角色管理**: 7个API（分页查询、详情、新增、更新、删除、分配权限、查询权限）
- **权限管理**: 6个API（分页查询、树形结构、详情、新增、更新、删除）
- **部门管理**: 7个API（分页查询、树形结构、详情、新增、更新、删除、获取部门用户）
- **任务管理**: 5个API（列表、暂停、恢复、触发、删除）
- **Demo管理**: 6个API（分页查询、详情、新增、更新、删除、批量删除）
- **总计**: 39个RESTful API端点

---

## 最佳实践

### 1. 分层原则
- ❌ **不要**: 在Controller中直接写数据库查询
- ✅ **应该**: Controller → Service → Repository
- ❌ **不要**: 跨层依赖，如Application直接依赖SqlSugar
- ✅ **应该**: 通过IRepository接口解耦

### 2. 异常处理
- ❌ **不要**: 吃掉异常 `catch { }`
- ✅ **应该**: 记录日志并抛出自定义异常

```csharp
try
{
    // 业务逻辑
}
catch (Exception ex)
{
    _logger.LogError(ex, "业务处理失败");
    throw new BusinessException("操作失败，请稍后重试");
}
```

### 3. 数据验证
- ❌ **不要**: 在Service中手动验证每个字段
- ✅ **应该**: 使用FluentValidation验证器
- ✅ **应该**: 在DTO级别验证，而不是Entity级别

### 4. 缓存使用
- ✅ **应该**: 缓存不常变化的数据（字典、配置、权限等）
- ❌ **不要**: 缓存频繁变化的数据
- ✅ **应该**: 设置合理的过期时间
- ✅ **应该**: 更新数据时同步删除缓存

### 5. 性能优化
- ✅ **应该**: 使用分页查询，避免全表扫描
- ✅ **应该**: 使用异步方法 `async/await`
- ✅ **应该**: 避免N+1查询，使用Join或Includes
- ✅ **应该**: 给常用查询字段建索引

### 6. 安全规范
- ✅ **应该**: 所有密码加密存储
- ✅ **应该**: 敏感接口添加权限验证
- ✅ **应该**: 生产环境使用HTTPS
- ✅ **应该**: 定期更新依赖包，修复安全漏洞

### 7. 日志规范
```csharp
// ❌ 不好的日志
_logger.LogInformation("用户登录");

// ✅ 好的日志
_logger.LogInformation("用户登录成功, UserId: {UserId}, IP: {IP}", userId, ipAddress);
```

### 8. 事务处理
```csharp
// 跨表操作使用事务
_repository.BeginTran();
try
{
    await _repository.InsertAsync(user);
    await _repository.InsertAsync(userRole);
    _repository.CommitTran();
}
catch
{
    _repository.RollbackTran();
    throw;
}
```

### 9. 开发约定
- ✅ 所有代码注释必须使用简体中文
- ✅ 禁止在 Common 层写业务逻辑
- ✅ Service 层不直接操作 HttpContext
- ✅ 所有I/O操作必须异步（async/await）
- ✅ DTO命名使用大驼峰（PascalCase）
- ✅ 接口必须通过统一返回格式
- ✅ 避免直接返回实体类，必须通过DTO转换

---

## 部署指南

### 1. 发布项目
```bash
dotnet publish -c Release -o ./publish
```

### 2. 配置生产环境
- 修改 `appsettings.Production.json`
- 使用强密码和复杂JWT密钥
- 开启HTTPS
- 配置Redis缓存
- 关闭详细错误信息

### 3. 部署方式
- **IIS**: 安装ASP.NET Core托管捆绑包
- **Linux + Nginx**: 使用systemd守护进程 + Nginx反向代理
- **Docker**: 使用官方.NET镜像
- **云服务**: Azure App Service / AWS Elastic Beanstalk / 阿里云等

### 4. 性能优化
- ✅ 启用Redis分布式缓存
- ✅ 配置连接池
- ✅ 启用响应压缩
- ✅ 使用CDN加速静态资源
- ✅ 数据库索引优化

---

## 项目统计

### 代码文件
- **实体类**: 9个（EntityBase, User, Role, Permission, RolePermission, UserRole, Department, OperationLog, Demo）
- **仓储类**: 2个（IRepository, SqlSugarRepository）
- **服务类**: 7个（UserService, RoleService, PermissionService, DepartmentService, JobService, DemoService, FileService）
- **控制器**: 6个（UserController, RoleController, PermissionController, DepartmentController, JobController, DemoController）
- **DTO类**: 18+个
- **验证器**: 6个
- **工具类**: 5个
- **配置类**: 8个
- **异常类**: 2个
- **缓存类**: 3个
- **定时任务**: 3个
- **中间件**: 2个
- **过滤器**: 2个

### 技术亮点

#### 1. 标准化实现
- ✅ 使用标准JWT库（System.IdentityModel.Tokens.Jwt）
- ✅ 遵循RBAC权限模型标准
- ✅ 采用中间件管道模式
- ✅ 使用Quartz.NET标准定时任务框架
- ✅ 统一的异常处理和返回格式

#### 2. 安全性
- ✅ JWT Token签名验证
- ✅ RefreshToken防重放
- ✅ 接口限流保护
- ✅ 权限细粒度控制
- ✅ 数据权限隔离
- ✅ 全局异常捕获

#### 3. 可维护性
- ✅ 模块化设计
- ✅ 依赖注入
- ✅ 配置化管理
- ✅ 完整的XML注释
- ✅ 统一的代码风格

#### 4. 可扩展性
- ✅ 中间件管道可扩展
- ✅ 过滤器可组合
- ✅ 定时任务可动态添加
- ✅ 权限模型可扩展
- ✅ 数据过滤器可定制

#### 5. 性能优化
- ✅ 滑动窗口限流算法
- ✅ 内存缓存优化
- ✅ 异步编程模式
- ✅ 批量操作支持

---

## 常用命令

```bash
# 构建项目
dotnet build

# 运行项目
dotnet run --project backendStd.Web.Entry

# 发布项目
dotnet publish -c Release

# 清理输出
dotnet clean

# 添加包
dotnet add package PackageName

# 数据库迁移（手动执行SQL或使用EnableInitDb自动建表）
```

---

## 后续扩展建议

### 已实现功能 ✅
- [x] **健康检查端点** - 完整的健康检查系统，监控数据库、Redis、Quartz、磁盘、内存等
- [x] **单元测试（xUnit）** - 完整的单元测试覆盖（UserService、JwtHandler、CacheService），25个测试全部通过
- [x] **容器化部署（Docker）** - 多阶段Dockerfile优化、Docker Compose完整环境
- [x] **Kubernetes部署** - 完整的K8s配置（Deployment、Service、Ingress、HPA、ConfigMap、Secret）
- [x] **CI/CD流水线** - GitHub Actions自动化（构建、测试、代码覆盖率、Docker镜像推送）

### 可选扩展功能
- [ ] 集成测试（WebApplicationFactory + SQLite内存数据库）
- [ ] API性能测试（BenchmarkDotNet）
- [ ] 分布式缓存（Redis集群支持）
- [ ] 消息队列（RabbitMQ/Kafka）
- [ ] 微服务支持（gRPC/Service Mesh）
- [ ] 性能监控（Application Insights / Prometheus + Grafana）

---

## 健康检查端点

### 功能说明

项目集成了完整的健康检查系统，监控各个关键组件的运行状态。

### 健康检查项

- ✅ **数据库连接** - MySQL连接状态检查
- ✅ **Redis缓存** - Redis连接和响应检查（可选）
- ✅ **Quartz调度器** - 定时任务调度器状态检查
- ✅ **磁盘空间** - 磁盘剩余空间检查
- ✅ **内存使用** - 进程内存使用情况检查
- ✅ **种子数据** - 数据库初始化状态检查

### API端点

```bash
# 基础健康检查（公开访问）
GET /health

# 详细健康报告（需要认证）
GET /api/health/details
```

### 配置说明

在 `appsettings.json` 中配置健康检查选项：

```json
{
  "HealthCheckOptions": {
    "Enabled": true,
    "CheckDatabase": true,
    "CheckRedis": false,
    "CheckQuartz": true,
    "CheckDiskSpace": true,
    "MinimumFreeDiskSpaceGB": 1,
    "CheckMemory": true,
    "MaxMemoryUsagePercentage": 90
  }
}
```

---

## 单元测试

### 测试覆盖

项目包含全面的单元测试，确保代码质量和可靠性：

- ✅ **UserService测试** - 8个测试用例（登录、注册、CRUD、权限验证）
- ✅ **JwtHandler测试** - 8个测试用例（Token生成、验证、Claims提取）
- ✅ **MemoryCacheService测试** - 9个测试用例（缓存CRUD、过期策略）

### 运行测试

```bash
# 运行所有单元测试
dotnet test backendStd.UnitTests

# 运行测试并生成覆盖率报告
dotnet test backendStd.UnitTests --collect:"XPlat Code Coverage"

# 运行特定测试
dotnet test backendStd.UnitTests --filter "FullyQualifiedName~UserServiceTests"
```

### 测试技术栈

- **xUnit** - 测试框架
- **Moq** - Mock框架
- **FluentAssertions** - 断言库

---

## Docker部署

### 快速启动

使用Docker Compose一键启动完整环境（API + MySQL + Redis + RabbitMQ）：

```bash
# 启动所有服务
docker-compose up -d

# 查看服务状态
docker-compose ps

# 查看日志
docker-compose logs -f api

# 停止所有服务
docker-compose down
```

### 服务访问

- **API服务**: http://localhost:5000
- **Swagger文档**: http://localhost:5000/swagger
- **健康检查**: http://localhost:5000/health
- **MySQL**: localhost:3306
- **Redis**: localhost:6379
- **RabbitMQ管理界面**: http://localhost:15672 (admin/rabbitmq123)
- **Nginx**: http://localhost:80

### 单独构建镜像

```bash
# 构建Docker镜像
docker build -t backendstd-api:latest .

# 运行容器
docker run -d -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e DbConnectionOptions__ConnectionConfigs__0__ConnectionString="your-connection-string" \
  backendstd-api:latest
```

---

## Kubernetes部署

### 部署步骤

1. **创建命名空间**（可选）

```bash
kubectl create namespace backendstd
```

2. **应用配置文件**

```bash
# 依次部署
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/pvc.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/ingress.yaml
kubectl apply -f k8s/hpa.yaml
```

3. **验证部署**

```bash
# 检查Pod状态
kubectl get pods -n default

# 检查服务状态
kubectl get svc -n default

# 查看部署详情
kubectl describe deployment backendstd-api

# 查看Pod日志
kubectl logs -f deployment/backendstd-api
```

### 水平扩缩容

项目配置了HPA（水平Pod自动扩缩容）：

- **最小副本数**: 2
- **最大副本数**: 10
- **CPU阈值**: 70%
- **内存阈值**: 80%

```bash
# 手动扩容
kubectl scale deployment backendstd-api --replicas=5

# 查看HPA状态
kubectl get hpa
```

### 配置更新

修改ConfigMap或Secret后，需要重启Pod：

```bash
kubectl rollout restart deployment/backendstd-api
```

---

## CI/CD流水线

### GitHub Actions工作流

项目包含三个自动化工作流：

#### 1. CI工作流 (`.github/workflows/ci.yml`)

**触发条件**: Push到main/develop分支 或 PR到main/develop分支

**执行步骤**:
- 代码检出
- .NET环境设置
- 依赖恢复
- 项目构建
- 单元测试执行
- 代码覆盖率生成
- 覆盖率上传到Codecov
- 代码格式检查
- 安全漏洞扫描

#### 2. CD工作流 (`.github/workflows/cd.yml`)

**触发条件**: Push到main分支 或 标签推送 或 手动触发

**执行步骤**:
- Docker镜像构建
- 推送到GitHub Container Registry
- （可选）部署到Kubernetes集群

#### 3. PR检查工作流 (`.github/workflows/pr-check.yml`)

**触发条件**: PR到main/develop分支

**执行步骤**:
- 代码构建
- 单元测试
- PR标题格式检查
- 文件大小检查
- 自动添加PR评论

### 使用CI/CD

```bash
# 1. 提交代码触发CI
git add .
git commit -m "feat: 添加新功能"
git push origin feature-branch

# 2. 创建PR触发PR检查
gh pr create --base main --head feature-branch

# 3. 合并到main触发CD
gh pr merge --merge

# 4. 创建版本标签触发发布
git tag v1.0.0
git push origin v1.0.0
```

### CI/CD状态徽章

可以在README中添加以下徽章：

```markdown
![CI](https://github.com/your-username/backend-dotnet-standard/workflows/CI/badge.svg)
![CD](https://github.com/your-username/backend-dotnet-standard/workflows/CD/badge.svg)
[![codecov](https://codecov.io/gh/your-username/backend-dotnet-standard/branch/main/graph/badge.svg)](https://codecov.io/gh/your-username/backend-dotnet-standard)
```

---

## 质量保证

- ✅ 代码编译成功（0 Errors）
- ✅ 符合C#命名规范
- ✅ 完整的XML注释
- ✅ 中文注释和文档
- ✅ 统一的代码风格
- ✅ 清晰的项目结构

---

## 贡献指南

欢迎提交Issue和Pull Request！

---

## 许可证

MIT License

---

**注意**: 这是一个模板项目，请根据实际业务需求进行扩展和定制。

---

**最后更新时间**: 2025年12月27日  
**版本**: v1.1  
**项目状态**: 生产就绪 + 扩展功能完善

### 更新日志

#### v1.1 (2025-12-27)
- ✅ 添加健康检查端点和自定义检查项
- ✅ 实现完整的单元测试框架（25个测试用例）
- ✅ 添加Docker容器化支持（多阶段构建）
- ✅ 完整的Kubernetes部署配置（7个配置文件）
- ✅ 实现CI/CD流水线（GitHub Actions）
- ✅ 更新文档，添加部署和测试指南

#### v1.0 (2025-12-25)
- 初始版本发布
- 完整的DDD四层架构
- JWT认证授权系统
- RBAC权限管理
- 定时任务框架
- 基础CRUD功能
