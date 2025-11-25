using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIA.S0.Domain.IntegrationEventLogs.Enums;

/// <summary>
/// 事件状态.
/// </summary>
public enum IntegrationEventLogState
{
    /// <summary>
    /// 出错.
    /// </summary>
    InError = -1,
    /// <summary>
    /// 待发布.
    /// </summary>
    Pending = 0,
    /// <summary>
    /// 处理中.
    /// </summary>
    InProgress = 1,
    /// <summary>
    /// 已发布.
    /// </summary>
    Published = 2
}
