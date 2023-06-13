using System;

using BDanMuLib.Emuns;

namespace BDanMuLib.Enums
{
    public class CoinType : BaseEnumeration
    {
        public static CoinType Silver = new CoinType(1, "银瓜子");
        public static CoinType Gold = new CoinType(2, "金瓜子");


        public CoinType(int id, string name) : base(id, name)
        {
        }

        public static CoinType CheckCoinType(string type)
        {
            if (type.Equals("gold", StringComparison.OrdinalIgnoreCase))
            {
                return Gold;
            }
            return Silver;
        }

    }
}
