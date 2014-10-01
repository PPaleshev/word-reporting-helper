using System.Threading;

namespace SampleWordHelper.Core.Common
{
    /// <summary>
    /// Контекст выполнения фоновых операций.
    /// </summary>
    public class BackgroundTaskContext
    {
        /// <summary>
        /// Флаг, равный 1, если запрошено завершение асинхронной задачи, в противном случае 0.
        /// </summary>
        int shutdownRequested = 0;

        /// <summary>
        /// Флаг, равный true, если запрошено завершение фоновой операции, иначе false.
        /// </summary>
        public bool ShutdownRequested
        {
            get { return Thread.VolatileRead(ref shutdownRequested) == 1; }
        }

        /// <summary>
        /// Устанавливает флаг завершения фоновой задачи.
        /// </summary>
        public void Shutdown()
        {
            Thread.VolatileWrite(ref shutdownRequested, 1);
        }
    }
}
