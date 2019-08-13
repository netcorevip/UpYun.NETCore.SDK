using System;
using System.Net.Http;

namespace UpYun.NETCore.SDK
{
    [Serializable]
    public class Result<T>
    {
        public bool State { get; set; } = true;

        public T Data { get; set; }

        public int Code { get; set; }

        public byte[] Bytes { get; set; }

        public HttpResponseMessage ResponseMessage { get; set; }

        public string Message { get; set; } = "";

        public void SetData(T obj)
        {
            this.Data = obj;
        }

        public Result() { }

        public Result(T t)
        {
            Data = t;
        }

        public Result<T> Error(string msg = "", int code = 0)
        {
            this.State = false;
            this.Message = msg;
            this.Code = code;
            return this;
        }

        /// <summary>
        /// 返回行数
        /// </summary>
        //  public int TotalCount { get; set; }

    }

    public class Result : Result<object>
    {

    }
}
