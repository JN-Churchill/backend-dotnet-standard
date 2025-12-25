using backendStd.Application.Dtos;
using backendStd.Application.Dtos.Demo;
using backendStd.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace backendStd.Application.Controllers;

/// <summary>
/// Demo控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly DemoService _demoService;

    public DemoController(DemoService demoService)
    {
        _demoService = demoService;
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    [HttpGet("page")]
    public async Task<PagedResult<DemoDto>> GetPaged([FromQuery] PageInput input)
    {
        return await _demoService.GetPagedAsync(input);
    }

    /// <summary>
    /// 根据ID获取
    /// </summary>
    [HttpGet("{id}")]
    public async Task<DemoDto> Get(long id)
    {
        return await _demoService.GetByIdAsync(id);
    }

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost]
    public async Task<long> Add([FromBody] DemoInput input)
    {
        return await _demoService.AddAsync(input);
    }

    /// <summary>
    /// 更新
    /// </summary>
    [HttpPut("{id}")]
    public async Task<bool> Update(long id, [FromBody] DemoUpdateInput input)
    {
        return await _demoService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        return await _demoService.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    [HttpDelete("batch")]
    public async Task<bool> BatchDelete([FromBody] List<long> ids)
    {
        return await _demoService.BatchDeleteAsync(ids);
    }
}
