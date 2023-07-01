using Microsoft.Data.SqlClient;

namespace DataHandlers;

internal class DataHandler
{
    // Database and base tables already created, but they're empty

    private const string PRODUCTS_TABLE_NAME = "Products";
    private const string PRODUCTS_FULL_NAME = PRODUCTS_TABLE_NAME + " (Name)";

    private const string PRODUCTS = "('Happy meal'), ('Car'), ('Air')";

    private const string CATEGORIES_TABLE_NAME = "Categories";
    private const string CATEGORIES_FULL_NAME = CATEGORIES_TABLE_NAME + " (Name)";

    private const string CATEGORIES = "('For kids'), ('Buyable')";

    private const string RELATIONS_TABLE_NAME = "Relations";
    private const string RELATIONS_FULL_NAME = RELATIONS_TABLE_NAME + " (CategoryName, ProductName)";

    private const string RELATIONS = "('For kids', 'Happy meal'), ('Buyable', 'Happy meal'), ('Buyable', 'Car')";

    public static async Task ConnectToSqlAsync()
    {
        string connectionString =
            $"Server=.\\SQLEXPRESS;" +
            $"Database=testDataBase;" +
            $"Trusted_Connection=True;" +
            $"Trust Server Certificate=true;";

        using SqlConnection connection = new(connectionString);

        await connection.OpenAsync();
        Console.WriteLine($"Connection opened: {connection.ClientConnectionId}");

        await RefreshDataAsync(connection);

        await DataConsoleWriter.WriteReadCommandAsync("Products", new($"SELECT * FROM {PRODUCTS_TABLE_NAME}", connection), valuesCount: 2);
        await DataConsoleWriter.WriteReadCommandAsync("Categories", new($"SELECT * FROM {CATEGORIES_TABLE_NAME}", connection), valuesCount: 2);
        await DataConsoleWriter.WriteReadCommandAsync("Relations", new($"SELECT * FROM {RELATIONS_TABLE_NAME}", connection), valuesCount: 3);

        List<object>[] table = await GetAllProductsWithRelations(connection);
        DataConsoleWriter.WriteTable("All products", table);

        await connection.CloseAsync();
    }

    private static async Task RefreshDataAsync(SqlConnection connection)
    {
        await RefreshTableAsync(connection, PRODUCTS_TABLE_NAME, PRODUCTS_FULL_NAME, PRODUCTS);
        await RefreshTableAsync(connection, CATEGORIES_TABLE_NAME, CATEGORIES_FULL_NAME, CATEGORIES);
        await RefreshTableAsync(connection, RELATIONS_TABLE_NAME, RELATIONS_FULL_NAME, RELATIONS);
    }

    private static async Task RefreshTableAsync(SqlConnection connection, string tableName, string fullName, string values)
    {
        await CommandExecuter.DeleteAllAsync(connection, tableName);
        await CommandExecuter.InsertAsync(connection, fullName, values);
    }

    private static async Task<List<object>[]> GetAllProductsWithRelations(SqlConnection connection)
    {
        List<object>[] productsWithoutRelations = await CommandExecuter.SelectAsync(new($"SELECT Name FROM {PRODUCTS_TABLE_NAME} WHERE Name NOT IN (SELECT ProductName FROM {RELATIONS_TABLE_NAME})", connection), valuesCount: 1);

        List<object>[] allProductsWithRelations = await CommandExecuter.SelectAsync(new($"SELECT CategoryName, ProductName FROM {RELATIONS_TABLE_NAME}", connection), valuesCount: 2);

        if (productsWithoutRelations.Length is 0 || productsWithoutRelations[0].Count is 0 or 1)
            return allProductsWithRelations;

        List<object>[] result = new List<object>[allProductsWithRelations.Length + productsWithoutRelations.Length - 1];

        for (int i = 0; i < allProductsWithRelations.Length; i++)
        {
            result[i] = new();

            for (int j = 0; j < allProductsWithRelations[i].Count; j++)
                result[i].Add(allProductsWithRelations[i][j]);
        }

        for (int i = 1; i < productsWithoutRelations[0].Count; i++)
        {
            result[0].Add("NULL \t");
            result[1].Add(productsWithoutRelations[0][i] + " \t");
        }

        return result;
    }
}
