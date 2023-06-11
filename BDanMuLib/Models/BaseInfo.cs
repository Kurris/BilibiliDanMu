using System;

namespace BDanMuLib.Models
{
    public abstract class BaseInfo
    {
        public string Key { get; } = Guid.NewGuid().ToString().Replace("-", string.Empty);
    }
}
