namespace EIA.S0.Domain.Core.Exceptions;

/// <summary>
/// 领域异常.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// 领域异常.
    /// </summary>
    /// <param name="message"></param>
    public DomainException(string message) : base(message)
    {
    }
}