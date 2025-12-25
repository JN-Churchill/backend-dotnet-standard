# Backend Template - Furion Api

## 🎯 目标
这是一个用于长期复用的企业级 Furion Api 后端模板，开箱即用。

## 🧱 技术栈
- **.NET 8** - 最新的.NET平台
- **Furion 4.9.6** - 现代化Web框架
- **SqlSugar 5.1.4.162** - 强大的ORM框架，支持MySQL/SqlServer/PostgreSQL
- **Serilog** - 结构化日志记录
- **StackExchange.Redis** - Redis缓存支持
- **Quartz.NET** - 任务调度框架
- **Mapster** - 高性能对象映射
- **FluentValidation** - 流畅的验证框架
- **Yitter.IdGenerator** - 雪花ID生成器
- **Swagger** - API文档自动生成

## 📂 项目结构

```
backendStd.sln
├── backendStd.Web.Entry          [启动层]
│   ├── Program.cs                 启动配置
│   ├── appsettings.json          配置文件
│   └── wwwroot/                  静态文件目录
│
├── backendStd.Web.Core           [Web核心层]
│   └── (中间件、过滤器、处理器等)
│
├── backendStd.Application        [应用层]
│   ├── Controllers/              控制器
│   │   ├── UserController.cs     用户管理
│   │   └── DemoController.cs     示例控制器
│   ├── Dtos/                     数据传输对象
│   │   ├── PageInput.cs          分页输入
│   │   ├── PagedResult.cs        分页结果
│   │   ├── User/                 用户DTO
│   │   └── Demo/                 演示DTO
│   ├── Services/                 业务服务
│   │   ├── UserService.cs        用户服务
│   │   ├── DemoService.cs        演示服务
│   │   └── FileService.cs        文件服务
│   └── Validators/               验证器
│       ├── UserValidator.cs      用户验证
│       └── DemoValidator.cs      演示验证
│
├── backendStd.Core               [核心层]
│   ├── Entity/                   实体模型
│   │   ├── Base/
│   │   │   ├── EntityBase.cs     实体基类(雪花ID、审计字段)
│   │   │   └── EntityTenantBase.cs  多租户基类
│   │   ├── User.cs               用户实体
│   │   └── Demo.cs               演示实体
│   ├── Repository/               仓储模式
│   │   ├── IRepository.cs        仓储接口
│   │   └── SqlSugarRepository.cs SqlSugar实现
│   ├── SqlSugarConfig/           数据库配置
│   │   └── SqlSugarSetup.cs      SqlSugar初始化
│   ├── Cache/                    缓存服务
│   │   ├── ICacheService.cs      缓存接口
│   │   ├── MemoryCacheService.cs 内存缓存
│   │   └── RedisCacheService.cs  Redis缓存
│   ├── Options/                  配置选项
│   │   ├── DbConnectionOptions.cs 数据库配置
│   │   ├── JWTSettingsOptions.cs JWT配置
│   │   ├── RedisOptions.cs       Redis配置
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
    ├── Models/
    │   └── UnifyResult.cs        (已废弃)
    └── Exceptions/
        ├── BusinessException.cs   业务异常
        └── ValidationException.cs 验证异常
```

## 🚀 快速开始

### 1. 环境要求
- .NET 8 SDK
- MySQL 5.7+ / SQL Server 2012+ / PostgreSQL 9.5+
- Redis (可选)

### 2. 配置数据库
编辑 `backendStd.Web.Entry/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=backendstd;Uid=root;Pwd=your_password;CharSet=utf8mb4;"
  }
}
```

### 3. 运行项目
```bash
cd backendStd.Web.Entry
dotnet run
```

### 4. 访问API文档
打开浏览器访问: `http://localhost:5000/swagger`

## 📋 功能特性

### ✅ 已实现
- [x] 4层架构设计（Entry/Web.Core/Application/Core/Common）
- [x] SqlSugar ORM集成，支持多数据库
- [x] 雪花ID主键生成策略
- [x] 实体基类（审计字段、软删除）
- [x] 仓储模式封装（CRUD、分页）
- [x] 统一返回结果格式
- [x] 内存缓存 & Redis缓存
- [x] Serilog日志记录（Console + File）
- [x] FluentValidation数据验证
- [x] Mapster对象映射
- [x] Swagger API文档
- [x] 用户管理示例（登录、CRUD）
- [x] Demo示例模块（完整CRUD）
- [x] 文件上传服务
- [x] MD5密码加密

### 🚧 待实现
- [ ] JWT认证完整实现
- [ ] RefreshToken刷新机制
- [ ] 权限管理（RBAC）
- [ ] 全局异常过滤器
- [ ] 请求日志中间件
- [ ] 接口限流中间件
- [ ] 数据权限过滤
- [ ] Quartz定时任务示例
- [ ] 多租户支持

## 🔧 核心功能说明

### 实体基类
所有实体继承自 `EntityBase`，自动获得：
- `Id` - 雪花ID主键
- `CreateTime` - 创建时间
- `UpdateTime` - 更新时间  
- `CreateUserId` - 创建人ID
- `UpdateUserId` - 更新人ID
- `IsDeleted` - 软删除标记
- `DeleteTime` - 删除时间

### 仓储模式
提供标准CRUD接口：
- `GetByIdAsync` - 根据ID查询
- `GetListAsync` - 列表查询
- `GetPagedAsync` - 分页查询
- `InsertAsync` - 新增
- `UpdateAsync` - 更新
- `DeleteAsync` - 物理删除
- `SoftDeleteAsync` - 软删除
- `BatchInsertAsync` - 批量新增

### 统一返回格式
```json
{
  "Code": 200,
  "Type": "success",
  "Message": null,
  "Result": { ... },
  "Extras": null,
  "Time": "2025-12-25T10:30:00"
}
```

## ⚠️ 开发约定
- ✅ 所有代码注释必须使用简体中文
- ✅ 禁止在 Common 层写业务逻辑
- ✅ Service 层不直接操作 HttpContext
- ✅ 所有I/O操作必须异步（async/await）
- ✅ DTO命名使用大驼峰（PascalCase）
- ✅ 接口必须通过统一返回格式
- ✅ 避免直接返回实体类，必须通过DTO转换

## 📝 API示例

### 用户登录
```bash
POST /api/user/login
Content-Type: application/json

{
  "UserName": "admin",
  "Password": "123456"
}
```

### 获取用户列表
```bash
GET /api/user/page?Page=1&PageSize=20
```

### 新增Demo
```bash
POST /api/demo
Content-Type: application/json

{
  "Name": "测试",
  "Description": "这是一个测试",
  "Sort": 1
}
```

## 🛠️ 技术说明

### 数据库初始化
项目启动时会自动：
1. 检测数据库是否存在，不存在则创建
2. 根据实体类自动创建表结构
3. 初始化必要的种子数据（如有配置）

### 缓存策略
- 默认使用内存缓存（MemoryCache）
- 可切换至Redis缓存（修改Program.cs注册）
- 提供统一的`ICacheService`接口

### 日志配置
- Console输出：开发环境彩色日志
- File输出：`logs/log-{Date}.txt` 按天滚动
- 可扩展至数据库、Elasticsearch等

## 📦 项目打包
```bash
dotnet publish -c Release -o ./publish
```

## 🤝 贡献指南
欢迎提交Issue和Pull Request！

## 📄 许可证
MIT License

---
**注意**: 这是一个模板项目，请根据实际业务需求进行扩展和定制。
