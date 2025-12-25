using backendStd.Application.Dtos.Demo;
using FluentValidation;

namespace backendStd.Common.Validators;

/// <summary>
/// Demo输入验证器
/// </summary>
public class DemoInputValidator : AbstractValidator<DemoInput>
{
    public DemoInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("名称不能为空")
            .Length(1, 100).WithMessage("名称长度必须在1-100个字符之间");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述最多500个字符");

        RuleFor(x => x.Sort)
            .GreaterThanOrEqualTo(0).WithMessage("排序号必须大于等于0");
    }
}

/// <summary>
/// Demo更新输入验证器
/// </summary>
public class DemoUpdateInputValidator : AbstractValidator<DemoUpdateInput>
{
    public DemoUpdateInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("名称不能为空")
            .Length(1, 100).WithMessage("名称长度必须在1-100个字符之间");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述最多500个字符");

        RuleFor(x => x.Status)
            .Must(x => x == 0 || x == 1).WithMessage("状态值必须为0或1");

        RuleFor(x => x.Sort)
            .GreaterThanOrEqualTo(0).WithMessage("排序号必须大于等于0");
    }
}
