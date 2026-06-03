namespace ReStyle.Helpers;

/// <summary>
/// Wraps Shell navigation for use in ViewModels.
/// </summary>
public static class NavigationService
{
    public static Task GoToAsync(string route) => Shell.Current.GoToAsync(route);
    public static Task GoToAsync(string route, IDictionary<string, object> parameters) =>
        Shell.Current.GoToAsync(route, parameters);
    public static Task GoBackAsync() => Shell.Current.GoToAsync("..");
}
