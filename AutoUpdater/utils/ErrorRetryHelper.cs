using System;

namespace AutoUpdater.utils
{

    /// <summary>
    /// 错误重试通用类
    /// </summary>
    public class ErrorRetryHelper
    {
        public static ReturnResult Handle(string errorRetryConfig, Func<bool> methed)
        {
            return Handle(errorRetryConfig, () =>
            {
                var istrue = methed();
                return istrue ? ReturnResult.SuccessResult() : ReturnResult.FailResult();
            });
        }
        /// <summary>
        /// 错误自动重试公用方法
        /// </summary>
        /// <param name="errorRetryConfig">重试策略</param>
        /// <param name="methed"></param>
        /// <returns></returns>
        public static ReturnResult Handle(string errorRetryConfig, Func<ReturnResult> methed)
        {
            if (string.IsNullOrEmpty(errorRetryConfig))
            {
                errorRetryConfig = "0";
            }

            string[] errTime = errorRetryConfig.Split(',');

            int errNum = 0;
            ReturnResult isTrue;
            do
            {
                try
                {
                    isTrue = methed();
                }
                catch (Exception ex)
                {
                    isTrue = ReturnResult.FailResult(ex.ToString());
                    System.Threading.Thread.Sleep(int.Parse(errTime[errNum]) * 1000);
                    errNum++;
                }
            } while (!isTrue.IsValid && errNum < errTime.Length);

            return isTrue;
        }
    }
}
