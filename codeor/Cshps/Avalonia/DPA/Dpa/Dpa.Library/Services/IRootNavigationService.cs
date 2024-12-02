namespace Dpa.Library.Services;

public interface IRootNavigationService
{
    void NavigateTo(string view);
}

/// <summary>
/// ��Ҫ���ڵ���Ϣ
/// </summary>
public static class ViewInfo
{
    public const string InitializationView = nameof(InitializationView);
    public const string MainView = nameof(MainView);
    
}