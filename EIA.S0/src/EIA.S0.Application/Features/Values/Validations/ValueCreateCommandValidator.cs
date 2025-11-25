using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIA.S0.Application.Features.Values.Commands;

namespace EIA.S0.Application.Features.Values.Validations;

/// <summary>
/// value create command validator.
/// </summary>
public class ValueCreateCommandValidator : AbstractValidator<ValueCreateCommand>
{
    /// <summary>
    /// 构造.
    /// </summary>
    public ValueCreateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id必填。")
            .MaximumLength(50).WithMessage("Id最长50个字符。")
            .Matches("^[a-zA-Z0-9-_]+$").WithMessage("只能是大写字母、小写字母、数字、短横、下划线。");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("名称必填。")
            .MaximumLength(100).WithMessage("最长100个字符。");
    }
}
