using System.Linq.Expressions;
using backendStd.Core.Entity.Base;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace backendStd.Core.Repository;

/// <summary>
/// SqlSugar仓储实现
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class SqlSugarRepository<T> : IRepository<T> where T : EntityBase, new()
{
    private readonly ISqlSugarClient _db;

    public SqlSugarRepository(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 根据ID查询实体
    /// </summary>
    public virtual async Task<T> GetByIdAsync(long id)
    {
        return await _db.Queryable<T>().InSingleAsync(id);
    }

    /// <summary>
    /// 根据条件查询实体列表
    /// </summary>
    public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression = null)
    {
        var query = _db.Queryable<T>();
        if (whereExpression != null)
        {
            query = query.Where(whereExpression);
        }
        return await query.ToListAsync();
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    public virtual async Task<(List<T> Items, int Total)> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>> whereExpression = null)
    {
        RefAsync<int> totalCount = 0;
        
        var query = _db.Queryable<T>();
        if (whereExpression != null)
        {
            query = query.Where(whereExpression);
        }

        var list = await query
            .OrderByDescending(x => x.CreateTime)
            .ToPageListAsync(page, pageSize, totalCount);

        return (list, totalCount);
    }

    /// <summary>
    /// 新增实体（返回雪花ID）
    /// </summary>
    public virtual async Task<long> InsertAsync(T entity)
    {
        return await _db.Insertable(entity).ExecuteReturnSnowflakeIdAsync();
    }

    /// <summary>
    /// 批量新增
    /// </summary>
    public virtual async Task<bool> BatchInsertAsync(List<T> entities)
    {
        return await _db.Insertable(entities).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    public virtual async Task<bool> UpdateAsync(T entity)
    {
        return await _db.Updateable(entity).ExecuteCommandHasChangeAsync();
    }

    /// <summary>
    /// 物理删除
    /// </summary>
    public virtual async Task<bool> DeleteAsync(long id)
    {
        return await _db.Deleteable<T>().In(id).ExecuteCommandHasChangeAsync();
    }

    /// <summary>
    /// 软删除
    /// </summary>
    public virtual async Task<bool> SoftDeleteAsync(long id)
    {
        return await _db.Updateable<T>()
            .SetColumns(x => new T { IsDeleted = true, DeleteTime = DateTime.Now })
            .Where(x => x.Id == id)
            .ExecuteCommandHasChangeAsync();
    }

    /// <summary>
    /// 批量软删除
    /// </summary>
    public virtual async Task<bool> SoftDeleteAsync(List<long> ids)
    {
        return await _db.Updateable<T>()
            .SetColumns(x => new T { IsDeleted = true, DeleteTime = DateTime.Now })
            .Where(x => ids.Contains(x.Id))
            .ExecuteCommandHasChangeAsync();
    }

    /// <summary>
    /// 执行SQL语句
    /// </summary>
    public virtual async Task<int> ExecuteSqlAsync(string sql, object parameters = null)
    {
        return await _db.Ado.ExecuteCommandAsync(sql, parameters);
    }
}
