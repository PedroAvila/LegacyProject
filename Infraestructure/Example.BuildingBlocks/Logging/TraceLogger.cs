using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Example.BuildingBlocks.Logging
{
    /// <summary>
    /// Adaptador mínimo de ILogger sobre System.Diagnostics.Trace para .NET Framework.
    /// Permite mantener la firma ILogger&lt;T&gt; de los behaviors sin arrastrar
    /// toda la pila de Microsoft.Extensions.Logging al proyecto legacy.
    /// </summary>
    public sealed class TraceLogger<T> : ILogger<T>
    {
        private static readonly string CategoryName = typeof(T).FullName;

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter != null
                ? formatter(state, exception)
                : state?.ToString();

            Trace.WriteLine(
                string.Format(
                    "[{0:yyyy-MM-dd HH:mm:ss}] [{1}] {2}: {3}",
                    DateTime.Now,
                    logLevel,
                    CategoryName,
                    message));

            if (exception != null)
            {
                Trace.WriteLine(exception);
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();

            public void Dispose()
            {
            }
        }
    }
}
