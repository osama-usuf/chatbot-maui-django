using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Serialization;
namespace MauiUI.Data;

// REST API Consuming Documentation: https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/rest?view=net-maui-8.0
public static class ItemAPI
{
    // TODO: Add fields for BaseAddress, Url, and authorizationKey


    // if android emulator, use 10.0.2.2 https://developer.android.com/studio/run/emulator-networking can be used, otherwise, use address of host device directly
    //static readonly string BaseAddress = "http://10.0.2.2:8000";
    static readonly string BaseAddress = "http://192.168.1.159:8000";
    static readonly string Url = $"{BaseAddress}/items/";

    static HttpClient client;

    static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

    // NOTE: We need to enable cleartext connections as well, following: https://devblogs.microsoft.com/xamarin/cleartext-http-android-network-security/

    private static async Task<HttpClient> GetClient()
    {
        if (client is not null)
        {
            return client;
        }
        client = new HttpClient();

        // TODO: authorization logic goes here
        // See https://www.youtube.com/watch?v=MxImRQEWe_w for example (1:02:00)

        client.DefaultRequestHeaders.Add("Accept", "application/json"); // accept responses in json format
        client.Timeout = TimeSpan.FromSeconds(3);
        return client;
    }

    public static async Task<(List<Item>, bool)> GetAsync()
    {
        var result = new List<Item>();
        bool success = false;
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No internet", "OK");
        }
        else
        {
            var client = await GetClient();
            try
            {
                // see: https://stackoverflow.com/questions/65383186/using-httpclient-getfromjsonasync-how-to-handle-httprequestexception-based-on
                // Method 1

                result = await client.GetFromJsonAsync<List<Item>>(Url);
                success = true;

                // Method 2
                //var response = await client.GetAsync(Url);
                //if (response.IsSuccessStatusCode)
                //{
                //    //string content = await response.Content.ReadAsStringAsync();
                //    //result = await client.GetFromJsonAsync<List<Item>>(Url);
                //    result = await response.Content.ReadFromJsonAsync<List<Item>>();
                //    success = true;
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
        return (result, success);
    }

    public static async Task<(List<Item>, bool)> AddAsync(string itemName)
    {
        var result = new List<Item>();
        bool success = false;
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No internet", "OK");
        }
        else
        {
            try
            {
                var client = await GetClient();
                Item item = new Item() { Name = itemName, Details = null };
                string json = JsonSerializer.Serialize<Item>(item, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Url, content);
                if (response.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
        return (result, success);
    }

    public static async Task<(List<Item>, bool)> Update(Item item)
    {
        var result = new List<Item>();
        bool success = false;
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No internet", "OK");
        }
        else
        {
            try
            {
                var client = await GetClient();
                string json = JsonSerializer.Serialize<Item>(item, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{Url}{item.Pk}/", content);
                if (response.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
        return (result, success);
    }

    public static async Task<bool> Delete(int itemPK)
    {
        bool success = false;
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("Uh Oh!", "No internet", "OK");
        }
        else
        {
            try
            {
                var client = await GetClient();

                var response = await client.DeleteAsync($"{Url}{itemPK}/");
                if (response.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
        return success;
    }
}