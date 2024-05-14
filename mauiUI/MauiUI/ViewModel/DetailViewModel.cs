using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiUI.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
namespace MauiUI.ViewModel;

[QueryProperty("Item", "Item")]
public partial class DetailViewModel : ObservableObject
{
    IConnectivity connectivity;

    [ObservableProperty]
    Item item;

    public DetailViewModel(IConnectivity connectivity)
    {
        this.connectivity = connectivity;
    }

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task Update()
    {
        var (result, success) = await ItemAPI.Update(Item);

        if (success)
        {
            WeakReferenceMessenger.Default.Send(new RefreshMessage(true));
            await GoBack();
        }
    }

    [RelayCommand]
    async Task Delete()
    {
        bool success = await ItemAPI.Delete(Item.Pk);

        if (success)
        {
            WeakReferenceMessenger.Default.Send(new RefreshMessage(true));
            await GoBack();
        }
    }
}
