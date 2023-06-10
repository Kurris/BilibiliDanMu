using System;

namespace BDanMuLib.Models
{
    public abstract class BaseInfo
    {
        public Guid Key { get; } = Guid.NewGuid();
    }
}
