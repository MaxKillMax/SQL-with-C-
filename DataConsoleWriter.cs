using Microsoft.Data.SqlClient;

namespace DataHandlers;

public static class DataConsoleWriter
{
    public static async Task WriteReadCommandAsync(string name, SqlCommand readCommand, int valuesCount)
    {
        Console.WriteLine($"\n{name}:");
        WriteTable(await CommandExecuter.SelectAsync(readCommand, valuesCount));
    }

    public static void WriteTable(string name, List<object>[] table)
    {
        Console.WriteLine($"\n{name}:");
        WriteTable(table);
    }

    private static void WriteTable(List<object>[] table)
    {
        if (table.Length == 0 || table[0].Count == 0)
        {
            Console.WriteLine("Is empty");
            return;
        }

        WriteLine(table.Length, (index) => table[index][0]);

        for (int i = 1; i < table[0].Count; i++)
            WriteLine(table.Length, (index) => table[index][i]);
    }

    private static void WriteLine(int length, Func<int, object> getWriteObject)
    {
        for (int i = 0; i < length; i++)
            Console.Write(getWriteObject?.Invoke(i) + " \t");

        Console.WriteLine();
    }
}