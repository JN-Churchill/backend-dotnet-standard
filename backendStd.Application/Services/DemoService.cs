using backendStd.Application.Dtos;
using backendStd.Application.Dtos.Demo;
using backendStd.Core.Entity;
using backendStd.Core.Repository;
using Mapster;

namespace backendStd.Application.Services;

/// <summary>
/// Demo服务
/// </summary>
public class DemoService
{
    private readonly IRepository<Demo> _demoRepository;

    public DemoService(IRepository<Demo> demoRepository)
    {
        _demoRepository = demoRepository;
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    public async Task<PagedResult<DemoDto>> GetPagedAsync(PageInput input)
    {
        var (items, total) = await _demoRepository.GetPagedAsync(input.Page, input.PageSize);
        
        return new PagedResult<DemoDto>
        {
            Items = items.Adapt<List<DemoDto>>(),
            Total = total,
            Page = input.Page,
            PageSize = input.PageSize
        };
    }

    /// <summary>
    /// 根据ID获取
    /// </summary>
    public async Task<DemoDto> GetByIdAsync(long id)
    {
        var demo = await _demoRepository.GetByIdAsync(id);
        if (demo == null)
            throw new Common.Exceptions.BusinessException("数据不存在");

        return demo.Adapt<DemoDto>();
    }

    /// <summary>
    /// 新增
    /// </summary>
    public async Task<long> AddAsync(DemoInput input)
    {
        var demo = input.Adapt<Demo>();
        return await _demoRepository.InsertAsync(demo);
    }

    /// <summary>
    /// 更新
    /// </summary>
    public async Task<bool> UpdateAsync(long id, DemoUpdateInput input)
    {
        var demo = await _demoRepository.GetByIdAsync(id);
        if (demo == null)
            throw new Common.Exceptions.BusinessException("数据不存在");

        // 映射更新字段
        input.Adapt(demo);

        return await _demoRepository.UpdateAsync(demo);
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        return await _demoRepository.SoftDeleteAsync(id);
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    public async Task<bool> BatchDeleteAsync(List<long> ids)
    {
        return await _demoRepository.SoftDeleteAsync(ids);
    }
}
