using System;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    public interface IConfigurationEditorPresenter: IDisposable
    {
        void OnSelectedProviderChanged(ListItem newListItem);
        void OnPropertyValueChanged();
    }
}