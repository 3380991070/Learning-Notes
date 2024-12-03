using Avalonia.Controls;
using Dpa.Library.Services;
using Dpa.Library.ViewModel;

namespace Dpa.Service;

/// <summary>
/// ����MainView��Content����
/// </summary>
public class RootNavigationService : IRootNavigationService
{
    /// <summary>
    /// ����MainView��Content����
    /// </summary>
    /// <param name="view"></param>
    public void NavigateTo(string view)
    {
        ServiceLocator SL = ServiceLocator.Current;
        if (view.Equals(ViewInfo.MainView))
        {
            SL.MainWindowModel.View = SL.MainViewModel;
            //SL.MainViewModel.SetViewAndClearStack(MenuNavigationConstant.ToDayView,ServiceLocator.Current.ToDayViewModel);
        }
        else
        {
            SL.MainWindowModel.View = SL.InitializationViewModel;
        }
    }
}