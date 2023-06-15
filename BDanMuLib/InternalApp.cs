using System;
using System.Collections.Concurrent;

namespace BDanMuLib
{
    public class InternalApp
    {
        /// <summary>
        /// 根服务提供器,对应dotnetcore在ConfigureServices中生成的IServiceProvider
        /// </summary>
        public static IServiceProvider ApplicationServices { get; set; }
    }
}
