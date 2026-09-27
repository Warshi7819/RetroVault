namespace RetroVaultWebApp.Models;

public class ListItem
{
    public int Id { get; set; }
    public string ListType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
