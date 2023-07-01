namespace DataHandlers;

internal class Program
{
    private static void Main(string[] _)
    {
        try
        {
#pragma warning disable CS4014
            DataHandler.ConnectToSqlAsync();
#pragma warning restore CS4014
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        Console.ReadKey();
    }
}
