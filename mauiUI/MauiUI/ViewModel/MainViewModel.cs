﻿using CommunityToolkit.Mvvm.ComponentModel;
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

        //ItemNames = new ObservableCollection<String>();
        Items = new ObservableCollection<Item>();

        // register a messenger so that we can refresh from other views
        // useful when the object is updated (altered, deleted, etc.) and the list on main page has to be updated
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

    //[ObservableProperty]
    //ObservableCollection<String> itemNames;

    [ObservableProperty]
    ObservableCollection<Item> items;

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
            //ItemNames.Clear();
            Items.Clear();
            foreach (Item item in itemsCollection)
            {
                Items.Add(item);
                //ItemNames.Add(item.Name);
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
    async Task Remove(int Pk)
    {
        bool success = await ItemAPI.Delete(Pk);

        if (success)
        {
            WeakReferenceMessenger.Default.Send(new RefreshMessage(true));
            await Refresh();
        }
    }

    [RelayCommand]
    async Task Tap(Item item)
    {
        var navParam = new Dictionary<string, object>()
        {
            { "Item", item }
        };
        //await Shell.Current.GoToAsync($"{nameof(DetailPage)}?Text={s}"); // seding simple query property, [QueryProperty] has to be added to destination view model for correct source gen
        await Shell.Current.GoToAsync(nameof(DetailPage), navParam); // seding simple query property
    }
}