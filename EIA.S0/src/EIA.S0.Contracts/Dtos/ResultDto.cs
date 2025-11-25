namespace EIA.S0.Contracts.Dtos;

/// <summary>
/// 返回结果.
/// </summary>
public class ResultDto
{
    /// <summary>
    /// 是否成功.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// 错误信息.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// 服务端错误.
    /// </summary>
    public bool IsServerError { get; init; }
    
    /// <summary>
    /// 成功.
    /// </summary>
    /// <returns></returns>
    public static ResultDto Success()
    {
        return new ResultDto { IsSuccess = true };
    }

    /// <summary>
    /// 错误.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ResultDto Fail(string message)
    {
        return new ResultDto { IsSuccess = false, Message = message };   
    }
    
    /// <summary>
    /// 服务端错误.
    /// </summary>
    /// <returns></returns>
    public static ResultDto Error()
    {
        return new ResultDto { IsSuccess = false, IsServerError = true, Message = "服务端发生错误." };
    }
}