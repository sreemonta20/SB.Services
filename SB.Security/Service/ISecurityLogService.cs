namespace SB.Security.Service
{
    /// <summary>
    /// It define the various logs. Where <see cref="SecurityLogService"/> implements this methods.
    /// </summary>
    /// <returns>interface</returns>
    public interface ISecurityLogService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void LogDebug(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void LogInfo(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void LogCritical(string message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void LogWarning(string message);
    }
}
