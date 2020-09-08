namespace BilibiliDanMuLib
{
    /// <summary>
    /// 日志输出委托
    /// </summary>
    /// <param name="e"></param>
    public delegate void LogMsg(LogArgs e);

    /// <summary>
    /// 日志参数
    /// </summary>
    public class LogArgs
    {
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Msg { get; internal set; }
    }
}
