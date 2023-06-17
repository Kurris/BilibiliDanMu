using System;

namespace LiveCore.Enums;

public class CoinType : BaseEnumeration
{
    public static readonly CoinType Silver = new(1, "银瓜子");
    public static readonly CoinType Gold = new(2, "金瓜子");


    public CoinType(int id, string name) : base(id, name)
    {
    }

    public static CoinType CheckCoinType(string type)
    {
        return type.Equals("gold", StringComparison.OrdinalIgnoreCase) ? Gold : Silver;
    }

}