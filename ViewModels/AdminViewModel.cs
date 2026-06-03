using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReStyle.Application.DTOs;
using ReStyle.Application.Interfaces;

namespace ReStyle.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly IUserService _userService;

    [ObservableProperty] private ObservableCollection<UserDto> _users = new();
    [ObservableProperty] private bool _isBusy;

    public AdminViewModel(IUserService userService) => _userService = userService;

    public async Task InitializeAsync()
    {
        IsBusy = true;
        var users = await _userService.GetAllUsersAsync();
        Users = new ObservableCollection<UserDto>(users);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task BlockUserAsync(UserDto user)
    {
        var (success, message) = user.IsBlocked
            ? await _userService.UnblockUserAsync(user.UserId)
            : await _userService.BlockUserAsync(user.UserId);

        if (success)
        {
            var idx = Users.IndexOf(user);
            if (idx >= 0)
                Users[idx] = user with { IsBlocked = !user.IsBlocked };
        }
        else await Shell.Current.DisplayAlert("Viga", message, "Sulge");
    }
}
