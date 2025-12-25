using System.Linq.Expressions;
using backendStd.Core.Entity.Base;

namespace backendStd.Core.Repository;

/// <summary>
/// 仓储接口
/// 定义标准的CRUD操作
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : EntityBase, new()
{
    /// <summary>
    /// 根据ID查询实体
    /// </summary>
    Task<T> GetByIdAsync(long id);

    /// <summary>
    /// 根据条件查询实体列表
    /// </summary>
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression = null);

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="whereExpression">查询条件</param>
    Task<(List<T> Items, int Total)> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>> whereExpression = null);

    /// <summary>
    /// 新增实体（返回雪花ID）
    /// </summary>
    Task<long> InsertAsync(T entity);

    /// <summary>
    /// 批量新增
    /// </summary>
    Task<bool> BatchInsertAsync(List<T> entities);

    /// <summary>
    /// 更新实体
    /// </summary>
    Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// 物理删除
    /// </summary>
    Task<bool> DeleteAsync(long id);

    /// <summary>
    /// 软删除
    /// </summary>
    Task<bool> SoftDeleteAsync(long id);

    /// <summary>
    /// 批量软删除
    /// </summary>
    Task<bool> SoftDeleteAsync(List<long> ids);

    /// <summary>
    /// 执行SQL语句
    /// </summary>
    Task<int> ExecuteSqlAsync(string sql, object parameters = null);
}
