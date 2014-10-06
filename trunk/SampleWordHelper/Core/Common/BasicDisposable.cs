using System;

namespace SampleWordHelper.Core.Common
{
    /// <summary>
    /// Базовый класс для обеспечения очистки ресурсов.
    /// </summary>
    public abstract class BasicDisposable : IDisposable
    {
        /// <summary>
        /// Флаг, равный true, если ресурсы объекта уже были очищены, иначе false.
        /// </summary>
        bool disposed;

        /// <summary>
        /// Флаг, равный true, если ресурсы объекта были освобождены, иначе false.
        /// </summary>
        public bool Disposed
        {
            get { return disposed; }
        }

        /// <summary>
        /// Финализатор.
        /// </summary>
        ~BasicDisposable()
        {
            if (disposed)
                return;
            DisposeSafe(false);
        }

        public void Dispose()
        {
            if (disposed)
                return;
            DisposeSafe(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Безопасно с точки зрения исключений очищает ресурсы.
        /// </summary>
        /// <param name="disposing">True, если метод вызывается при вызове <see cref="Dispose"/>, и false, если из финализатора.</param>
        void DisposeSafe(bool disposing)
        {
            disposed = true;
            if (disposing)
                SafeDisposeManaged();
            SafeDisposeUnmanaged();
        }

        /// <summary>
        /// Безопасно с точки зрения исключений очищает управляемые ресурсы.
        /// </summary>
        void SafeDisposeManaged()
        {
            try
            {
                DisposeManaged();
            }
            catch
            {
                //TODO PP: Log disposal error
            }
        }

        /// <summary>
        /// Безопасно с точки зрения исключений очищает неуправляемые ресурсы.
        /// </summary>
        void SafeDisposeUnmanaged()
        {
            try
            {
                DisposeUnmanaged();
            }
            catch
            {
                //TODO PP: Log disposal error
            }
        }

        /// <summary>
        /// Вызывается для очистки управляемых ресурсов.
        /// </summary>
        protected abstract void DisposeManaged();

        /// <summary>
        /// Вызывается для очистки неуправляемых ресурсов.
        /// </summary>
        protected virtual void DisposeUnmanaged()
        {
        }
    }

    /// <summary>
    /// Вспомогательные методы.
    /// </summary>
    public static class BasicDisposableExt
    {
        /// <summary>
        /// Выполняет безопасную с точки зрения исключений очистку ресурсов.
        /// </summary>
        /// <param name="disposable">Объект.</param>
        public static void SafeDispose(this IDisposable disposable)
        {
            try
            {
                if (disposable != null)
                    disposable.Dispose();
            }
            catch
            {
                //TODO PP: Log disposing error
            }
        }
    }
}
