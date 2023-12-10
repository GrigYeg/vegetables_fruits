using Microsoft.Data.Sqlite;
using System;
using System.Text;

internal class Program
{
    private static string ConnectionString = "Data Source=vegetables_fruits.sqlite;";

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        using var connection = new SqliteConnection(ConnectionString);

        connection.Open();
        CreateTable(connection);
        CreateItem(connection);

        ReadAndDisplayAll(connection, "SELECT * FROM vegetables_fruits", "Усі дані про фрукти та овочі");
        ReadAndDisplayAll(connection, "SELECT name FROM vegetables_fruits", "Всі назви овочів і фруктів");
        ReadAndDisplayAll(connection, "SELECT color FROM vegetables_fruits", "Всі назви кольорів");
        ReadAndDisplayAll(connection, "SELECT MIN(calorie) FROM vegetables_fruits", "Мінімальна калорійність");
        ReadAndDisplayAll(connection, "SELECT MAX(calorie) FROM vegetables_fruits", "Максимальна калорійність");
        ReadAndDisplayAll(connection, "SELECT AVG(calorie) FROM vegetables_fruits", "Середня калорійність");
        ReadAndDisplayAll(connection, "SELECT COUNT(*) FROM vegetables_fruits WHERE type =\"Овоч\" ", "Кількість овочів");
        ReadAndDisplayAll(connection, "SELECT COUNT(*) FROM vegetables_fruits WHERE type =\"Фрукт\" ", "Кількість фруктів");
        ReadAndDisplayAll(connection, "SELECT COUNT(*) FROM vegetables_fruits WHERE color =\"Помаранчевий\" ", "Кількість овочів і фруктів помаранчевого кольору");
        ReadAndDisplayAll(connection, "SELECT color,  COUNT(*) AS count FROM vegetables_fruits GROUP BY color", "Кількість овочів і фруктів кожного кольору");
        ReadAndDisplayAll(connection, "SELECT name, calorie FROM vegetables_fruits WHERE calorie < 50", "Овочі і фрукти з калорійністю нижче 50");
        ReadAndDisplayAll(connection, "SELECT name, calorie FROM vegetables_fruits WHERE calorie > 50", "Овочі і фрукти з калорійністю вище 50");
        ReadAndDisplayAll(connection, "SELECT name, calorie FROM vegetables_fruits WHERE calorie > 15 AND calorie < 60", "Овочі і фрукти з калорійністю в діапазоні від 15 до 60");
        ReadAndDisplayAll(connection, "SELECT name FROM vegetables_fruits WHERE color =\"Червоний\" ", "Овочі і фрукти червоного кольору");
        ReadAndDisplayAll(connection, "SELECT name FROM vegetables_fruits WHERE color =\"Жовтий\" ", "Овочі і фрукти жовтого кольору");

        Console.ReadKey();
    }
 
    private static void CreateItem(SqliteConnection connection)
    {
        using var transaction = connection.BeginTransaction();
        string[,] items =
        {
            {"Яблуко", "Фрукт", "Червоний", "52"},
            {"Томат", "Овоч", "Червоний", "18"},
            {"Груша", "Фрукт", "Жовтий", "57"},
            {"Авокадо", "Фрукт", "Зелений", "160"},
            {"Огірок", "Овоч", "Зелений", "15"},
            {"Картопля", "Овоч", "Коричневий", "74"},
            {"Морква", "Овоч", "Помаранчевий", "41"},
            {"Цибуля", "Овоч", "Білий", "40"},
            {"Мандарин", "Фрукт", "Помаранчевий", "53"},
            {"Кукуруза", "Овоч", "Жовтий", "101"},
            {"Банан", "Фрукт", "Жовтий", "89"},
            {"Персик", "Фрукт", "Помаранчевий", "39"},
        };

        for (int i = 0; i < items.GetLength(0); i++)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO vegetables_fruits (name, type, color, calorie) VALUES ($name, $type, $color, $calorie)";

            string name = items[i, 0];
            string type = items[i, 1];
            string color = items[i, 2];
            int calorie = int.Parse(items[i, 3]);

            insertCommand.Parameters.AddWithValue("$name", name);
            insertCommand.Parameters.AddWithValue("$type", type);
            insertCommand.Parameters.AddWithValue("$color", color);
            insertCommand.Parameters.AddWithValue("$calorie", calorie);

            insertCommand.ExecuteNonQuery();
        }
        transaction.Commit();
    }
    private static void ReadAndDisplayAll(SqliteConnection connection, string comm, string displayMessage)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = comm;

        using var reader = command.ExecuteReader();
        Console.WriteLine();
        Console.WriteLine(displayMessage);
        Console.WriteLine();

        while (reader.Read())
        {
            string output = "";
            for (int i = 0; i < reader.FieldCount; i++)
            {
                output += $"{reader.GetString(i),-15}\t";
            }
            Console.WriteLine(output);
        }
    }
    private static void ExecuteNonQuery(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void CreateTable(SqliteConnection connection)
    {
        string sql = "CREATE TABLE IF NOT EXISTS vegetables_fruits (name varchar(255), type varchar(255), color varchar(255), calorie int)";
        ExecuteNonQuery(connection, sql);
    }
}