using System;

namespace SampleWordHelper.Core.Common
{
    /// <summary>
    /// Заглушка для IDisposable.
    /// </summary>
    public sealed class EmptyDisposable: IDisposable
    {
        public void Dispose()
        {
        }
    }
}