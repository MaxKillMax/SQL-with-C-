using Microsoft.Data.SqlClient;

namespace DataHandlers;

public static class CommandExecuter
{
    /// <summary>
    /// 0 index of each list it's a title of column
    /// </summary>
    /// <param name="command"></param>
    /// <param name="valuesCount"></param>
    /// <returns></returns>
    public static async Task<List<object>[]> SelectAsync(SqlCommand command, int valuesCount)
    {
        using SqlDataReader reader = await command.ExecuteReaderAsync();
        List<object>[] objectsList = new List<object>[valuesCount];

        for (int i = 0; i < objectsList.Length; i++)
            objectsList[i] = new();

        if (reader.HasRows)
        {
            for (int i = 0; i < objectsList.Length; i++)
                objectsList[i].Add(reader.GetName(i));

            while (await reader.ReadAsync())
            {
                for (int i = 0; i < objectsList.Length; i++)
                    objectsList[i].Add(reader.GetValue(i));
            }
        }

        await reader.CloseAsync();
        return objectsList;
    }

    public static async Task DeleteAllAsync(SqlConnection connection, string table) => await ExecuteNonQueryCommandAsync(connection, $"DELETE FROM {table}");

    public static async Task InsertAsync(SqlConnection connection, string table, string values) => await ExecuteNonQueryCommandAsync(connection, $"INSERT INTO {table} VALUES {values}");

    private static async Task ExecuteNonQueryCommandAsync(SqlConnection connection, string text)
    {
        SqlCommand command = new(text, connection);
        await command.ExecuteNonQueryAsync();
    }
}
