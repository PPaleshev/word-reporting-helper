using System;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Интерфейс состояния поставщика каталога.
    /// </summary>
    public interface IProviderState
    {
        void Initialize();
        void Shutdown();
    }

    public class InactiveState : IProviderState
    {
        public void Initialize()
        {
            throw new InvalidOperationException("unable to initialize inactive state");
        }

        public void Shutdown()
        {
            throw new InvalidOperationException("unable to shutdown inactive state");
        }
    }

    /// <summary>
    /// Активное состояние провайдера.
    /// </summary>
    public class ActiveState : IProviderState
    {
        /// <summary>
        /// Параметры работы текущего провайдера.
        /// </summary>
        readonly FileSystemProviderSettings settings;

        public ActiveState(FileSystemProviderSettings settings)
        {
            this.settings = settings;
        }

        public void Initialize()
        {
        }

        public void Shutdown()
        {
        }
    }
}