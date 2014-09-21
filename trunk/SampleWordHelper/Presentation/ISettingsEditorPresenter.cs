using System;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    public interface ISettingsEditorPresenter: IDisposable
    {
        void OnSelectedProviderChanged(Item<string> newItem);
        void OnPropertyValueChanged();
    }
}