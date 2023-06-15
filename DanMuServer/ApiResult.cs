using System.Data;

namespace DanMuServer
{

    /// <summary>
    /// api result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        public int Code { get; set; } = 200;
        public string Message { get; set; } = "success";
        public T Data { get; set; }
    }
}
