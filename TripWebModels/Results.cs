namespace TripWebData
{
    /// <summary>
    /// 统一数据响应格式
    /// </summary>
    public class Results<T>
    {
        /// <summary>
        /// 自定义的响应码，可以和http响应码一致，也可以不一致
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 中文消息提示
        /// </summary>
        public string? Msg { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 响应的数据
        /// </summary>
        public T? Data { get; set; }
        /// <summary>
        /// 返回的Token: 如果有值，则前端需要此这个值替旧的token值
        /// </summary>
        public string? Token { get; set; }
        /// <summary>
        /// 设置数据的结果
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static Results<T> DataResult(T data)
        {
            return new Results<T>
            {
                Code = 1,
                Data = data,
                Msg = "请求成功",
                Success =
            true
            };
        }
        /// <summary>
        /// 响应成功的结果
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Results<T> SuccessResult(string msg = "操作成功")
        {
            return new Results<T>
            {
                Code = 1,
                Data = default,
                Msg = msg,
                Success =
            true
            };
        }
        /// <summary>
        /// 响应失败的结果
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Results<T> FailResult(string msg = "请求失败")
        {
            return new Results<T>
            {
                Code = -1,
                Data = default,
                Msg = msg,
                Success =
            false
            };
        }
        /// <summary>
        /// 参数有误
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Results<T> InValidParameter(string msg = "参数有误")
        {
            return new Results<T>
            {
                Code = -1,
                Data = default,
                Msg = msg,
                Success =
            false
            };
        }
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static Results<T> GetResult(int code = 0, string? msg = null, T? data =
        default, bool success = true)
        {
            return new Results<T>
            {
                Code = code,
                Data = data,
                Msg = msg,
                Success =
            success
            };
        }
        /// <summary>
        /// 设置token结果
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Results<T> TokenResult(string token)
        {
            return new Results<T>
            {
                Code = 1,
                Data = default,
                Msg = "请求成功",
                Success = true,
                Token = token
            };
        }
    }
}
