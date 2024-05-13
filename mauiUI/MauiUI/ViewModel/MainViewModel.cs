using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiUI.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace MauiUI.ViewModel;

public partial class MainViewModel : ObservableObject
{
    IConnectivity connectivity;
    public MainViewModel(IConnectivity connectivity)
    {
        this.connectivity = connectivity;

        Items = new ObservableCollection<String>();

        WeakReferenceMessenger.Default.Register<RefreshMessage>(this, async (r, m) =>
        {
            await LoadData();
        });

        Task.Run(LoadData);
    }

    [ObservableProperty]
    string text;

    // post API integration
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisplayReady))]
    bool _isRefreshing = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisplayReady))]
    bool _isBusy = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisplayReady))]
    bool _fetchError = false;

    public bool IsDisplayReady => !IsBusy && !IsRefreshing && !FetchError;

    [ObservableProperty]
    bool _addError = false;

    [ObservableProperty]
    ObservableCollection<String> items;

    [RelayCommand]
    async Task LoadData()
    {
        FetchError = false;

        if (IsBusy)
            return;

        IsRefreshing = true;
        IsBusy = true;

        var (itemsCollection, success) = await ItemAPI.GetAsync();

        FetchError = !success;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Items.Clear();
            foreach (Item item in itemsCollection)
            {
                Items.Add(item.Name);
            }
        });

        IsRefreshing = false;
        IsBusy = false;
    }

    [RelayCommand]
    async Task Refresh()
    {
        IsRefreshing = true;
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await LoadData();
        });
        IsRefreshing = false;
    }

    [RelayCommand]
    async Task Add()
    {
        if (string.IsNullOrEmpty(Text))
        {
            // disallow empty adds
            return;
        }

        var (itemsCollection, success) = await ItemAPI.AddAsync(Text);

        AddError = !success;

        if (success)
        {
            //items.Add(Text);
            await Refresh();
        }
    }


    [RelayCommand]
    void Remove(string s)
    {
        if (Items.Contains(s))
        {
            Items.Remove(s);
        }
    }

    [RelayCommand]
    async Task Tap(string s)
    {
        await Shell.Current.GoToAsync($"{nameof(DetailPage)}?Text={s}"); // seding simple query property
    }

}