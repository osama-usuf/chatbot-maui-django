namespace MauiUI.Data;

[Serializable]
public class Item
{
    public int Pk { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }

    // everything that will be present in the API's returned JSON should be defined here one-to-one for deserialization to work correctly
    public override string ToString()
    {
        return Name + " " + Details;
    }
}