namespace Hunter_Industries_API_Control_Panel.Services
{
    public class LoggerService
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [INFO] {message}");
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [WARN] {message}");
        }

        public void LogError(string message, Exception? ex = null)
        {
            Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [ERROR] {message}");

            if (ex != null)
            {
                Console.WriteLine($"  Exception: {ex.Message}");
                Console.WriteLine($"  StackTrace: {ex.StackTrace}");
            }
        }
    }
}
