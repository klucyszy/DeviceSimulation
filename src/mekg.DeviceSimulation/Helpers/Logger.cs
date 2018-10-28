using Microsoft.Extensions.Logging;
using System;

namespace mekg.DeviceSimulation.Helpers
{
    public static class Logger
    {
        public static void WriteInfo(ILogger logger, string info, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[App.cs] {info}");
            Console.ResetColor();
            logger.LogInformation($"[App.cs] {info}");
        }

        public static void WriteError(ILogger logger, string info)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[App.cs] {info}");
            Console.ResetColor();
            logger.LogError($"[App.cs] {info}");
        }
    }
}
