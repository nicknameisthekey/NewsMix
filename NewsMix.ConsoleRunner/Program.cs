namespace NewsMix.ConsoleRunner;
public class Program
{
    public static async Task Main()
    {
        var nc = new NoobClubFeed();
        var items = await nc.GetItems();
        foreach (var item in items)
        {
            System.Console.WriteLine($"Title: {item.Text}");
            System.Console.WriteLine($"Url: {item.Url}");
            System.Console.WriteLine($"Type: {item.Type}");
            System.Console.WriteLine($"----------------------");
            System.Console.WriteLine();
            System.Console.WriteLine();
        }
    }
}
