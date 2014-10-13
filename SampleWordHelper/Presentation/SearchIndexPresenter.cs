using System.Threading;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Indexation;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер представления для заполнения индекса.
    /// </summary>
    public class SearchIndexPresenter : BasicDisposable
    {
        /// <summary>
        /// Контекст времени исполнения.
        /// </summary>
        readonly IRuntimeContext context;

        /// <summary>
        /// Представление для отображения прогресса.
        /// </summary>
        readonly IWaitingView view;

        /// <summary>
        /// Создаёт новый экземпляр менеджера заполнения индекса.
        /// </summary>
        /// <param name="context">Контекст времени исполнения.</param>
        public SearchIndexPresenter(IRuntimeContext context)
        {
            this.context = context;
            view = context.ViewFactory.CreateWaitingView();
        }

        /// <summary>
        /// Начинает выполнение индексации.
        /// </summary>
        /// <param name="catalog">Экземпляр индексируемого каталога.</param>
        /// <param name="searchEngine">Экземпляр поискового механизма.</param>
        public void Run(ICatalog catalog, SearchEngine searchEngine)
        {
            context.Application.ScreenUpdating = false;
            try
            {
                new Thread(view.Show).Start();
                if (searchEngine.IsCreated)
                    searchEngine.UpdateIndex(catalog, view);
                else
                    searchEngine.BuildIndex(catalog, view);
            }
            finally
            {
                view.Close();
                context.Application.ScreenUpdating = true;
                context.Application.ScreenRefresh();
            }
        }

        protected override void DisposeManaged()
        {
            view.SafeDispose();
        }
    }
}
