namespace SB.Security.Service
{
    /// <summary>
    /// It implements various logs which defined by <see cref="ISecurityLogService"/>.
    /// </summary>
    public class SecurityLogService: ISecurityLogService
    {
        private readonly ILogger<SecurityLogService> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <returns>void</returns>
        public SecurityLogService(ILogger<SecurityLogService> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Critical
        /// </summary>
        /// <param name="message"></param>
        /// <returns>void</returns>
        public void LogCritical(string message)
        {
            this._logger?.LogCritical(message);
        }
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="message"></param>
        /// <returns>void</returns>
        public void LogDebug(string message)
        {
            this._logger?.LogDebug(message);
        }
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        public void LogError(string message)
        {
            this._logger?.LogError(message);
        }
        /// <summary>
        /// Info
        /// </summary>
        /// <param name="message"></param>
        /// <returns>void</returns>
        public void LogInfo(string message)
        {
            this._logger?.LogInformation(message);
        }
        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message"></param>
        /// <returns>void</returns>
        public void LogWarning(string message)
        {
            this._logger?.LogWarning(message);
        }
    }
}
