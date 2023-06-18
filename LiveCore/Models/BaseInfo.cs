using System;

namespace LiveCore.Models;

public abstract class BaseInfo
{
    public string Key { get; } = Guid.NewGuid().ToString().Replace("-", string.Empty);
}