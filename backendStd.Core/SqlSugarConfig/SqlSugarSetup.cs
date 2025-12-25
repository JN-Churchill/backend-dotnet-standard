using backendStd.Core.Entity.Base;
using backendStd.Core.Options;
using Furion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Yitter.IdGenerator;

namespace backendStd.Core.SqlSugarConfig;

/// <summary>
/// SqlSugar配置和服务注册
/// </summary>
public static class SqlSugarSetup
{
    /// <summary>
    /// 注册SqlSugar服务
    /// </summary>
    public static IServiceCollection AddSqlSugarSetup(this IServiceCollection services)
    {
        // 读取配置
        var dbOptions = App.GetOptions<DbConnectionOptions>();
        var snowIdOptions = App.GetOptions<SnowIdOptions>();

        // 初始化雪花ID生成器
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions
        {
            WorkerId = snowIdOptions?.WorkerId ?? 1
        });

        // 配置SqlSugar
        var configList = new List<SqlSugar.ConnectionConfig>();
        
        foreach (var config in dbOptions.ConnectionConfigs)
        {
            var connectionConfig = new SqlSugar.ConnectionConfig
            {
                ConfigId = config.ConfigId,
                ConnectionString = config.ConnectionString,
                DbType = ParseDbType(config.DbType),
                IsAutoCloseConnection = config.IsAutoCloseConnection,
                InitKeyType = InitKeyType.Attribute,
                
                // AOP配置
                AopEvents = new AopEvents
                {
                    // SQL执行前
                    OnLogExecuting = (sql, pars) =>
                    {
                        if (config.EnableSqlLog)
                        {
                            Console.WriteLine($"[SQL执行] {sql}");
                            if (pars != null && pars.Length > 0)
                            {
                                Console.WriteLine($"[参数] {string.Join(",", pars.Select(p => $"{p.ParameterName}={p.Value}"))}");
                            }
                        }
                    },
                    
                    // 慢SQL记录（>1秒）
                    OnLogExecuted = (sql, pars) =>
                    {
                        // 可以在这里记录慢SQL到日志
                    },

                    // 数据审计
                    DataExecuting = (oldValue, entityInfo) =>
                    {
                        if (entityInfo.OperationType == DataFilterType.InsertByObject)
                        {
                            // 新增操作
                            if (entityInfo.EntityValue is EntityBase entity)
                            {
                                if (entity.Id == 0)
                                {
                                    entity.Id = YitIdHelper.NextId();
                                }
                                entity.CreateTime = DateTime.Now;
                                // TODO: 从HttpContext获取当前用户ID
                                // entity.CreateUserId = CurrentUserId;
                            }
                        }
                        else if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                        {
                            // 更新操作
                            if (entityInfo.EntityValue is EntityBase entity)
                            {
                                entity.UpdateTime = DateTime.Now;
                                // TODO: 从HttpContext获取当前用户ID
                                // entity.UpdateUserId = CurrentUserId;
                            }
                        }
                    }
                },
                
                // 更多配置
                MoreSettings = new ConnMoreSettings
                {
                    IsAutoRemoveDataCache = true, // 自动清理缓存
                    SqlServerCodeFirstNvarchar = true // SqlServer默认使用nvarchar
                },
                
                // 差异日志
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (type, column) =>
                    {
                        // 可以在这里配置列的默认值等
                    }
                }
            };
            
            configList.Add(connectionConfig);
        }

        // 注册SqlSugarClient
        services.AddSingleton<ISqlSugarClient>(provider =>
        {
            var db = new SqlSugarScope(configList, db =>
            {
                // 全局过滤器
                // 软删除过滤器
                db.QueryFilter.AddTableFilter<EntityBase>(u => u.IsDeleted == false);
                
                // 数据权限过滤器（需要配合HttpContext使用）
                // 注意：这里只是示例，实际使用时需要在Repository中根据用户的数据权限动态添加过滤条件
                // 例如：db.QueryFilter.AddTableFilter<EntityBase>(u => u.CreateUserId == currentUserId);
                
                // 租户过滤器（需要配合HttpContext使用）
                // 对于继承EntityTenantBase的实体，自动过滤租户数据
                // 注意：实际使用时需要在Repository中根据当前租户ID动态添加过滤条件
                // 例如：db.QueryFilter.AddTableFilter<EntityTenantBase>(u => u.TenantId == currentTenantId);
            });

            return db;
        });

        // 初始化数据库
        InitDatabase(services);

        return services;
    }

    /// <summary>
    /// 初始化数据库（创建表、种子数据）
    /// </summary>
    private static void InitDatabase(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var db = serviceProvider.GetRequiredService<ISqlSugarClient>();
        var dbOptions = App.GetOptions<DbConnectionOptions>();

        foreach (var config in dbOptions.ConnectionConfigs)
        {
            if (config.EnableInitDb)
            {
                // 获取当前程序集中的所有实体类型
                var entityTypes = App.EffectiveTypes
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(EntityBase)))
                    .ToArray();

                // 创建表（CodeFirst）
                db.DbMaintenance.CreateDatabase();
                db.CodeFirst.InitTables(entityTypes);

                Console.WriteLine($"[数据库初始化] 已创建 {entityTypes.Length} 张表");
            }
        }
    }

    /// <summary>
    /// 解析数据库类型
    /// </summary>
    private static DbType ParseDbType(string dbType)
    {
        return dbType?.ToLower() switch
        {
            "mysql" => DbType.MySql,
            "sqlserver" => DbType.SqlServer,
            "postgresql" => DbType.PostgreSQL,
            "sqlite" => DbType.Sqlite,
            "oracle" => DbType.Oracle,
            _ => DbType.MySql
        };
    }
}
