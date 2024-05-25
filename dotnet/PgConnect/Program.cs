using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.IO;
using System.Threading.Tasks;


class Program
{
    static async Task Main()
    {
        string logFilePath = Environment.GetEnvironmentVariable("LOG_FILE_PATH");
        /*string dirName = Path.GetDirectoryName(logFilePath);
        if (!Path.Exists(dirName)){
            Directory.CreateDirectory(dirName);
        }*/
        string intervalString = Environment.GetEnvironmentVariable("INTERVAL_MINUTES");
        int intervalMinutes = string.IsNullOrEmpty(intervalString) ? 5 : int.Parse(intervalString);

       
        using (StreamReader sr = new StreamReader("sdl.conf"))
            {
                Console.WriteLine("Имя пользователя?");
                var userName = Console.ReadLine();

                Console.WriteLine("Пароль?");
                var password = Console.ReadLine();
                Console.WriteLine($"Добро пожаловать, {userName}");
                var connectionString = await sr.ReadLineAsync();
                Npgsql.NpgsqlConnectionStringBuilder csb = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
                csb.Username = userName;
                csb.Password = password;
                using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                                {
                                     await logWriter.WriteAsync($"Успешное подключение к базе данных в {DateTime.Now}");
                                }

                while (true) 
                {
                    try
                    {
                        using (var connection = new NpgsqlConnection(csb.ConnectionString))
                        {
                        
                        await connection.OpenAsync();
                        Console.WriteLine($"Успешное подключение к базе данных в {DateTime.Now}");

                        using (var cmd = new NpgsqlCommand("SELECT VERSION();", connection))
                        {
                            await Menu(connection);
                        }
                        
                        }
                    }
                    catch (Exception ex)
                        {
                        Console.Error.WriteLine($"Ошибка при подключении к базе данных в {DateTime.Now}");
                        using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                                {
                                    Console.WriteLine("ТУТ");
                                    logWriter.WriteLine($"Ошибка при подключении к базе данных в {DateTime.Now}: {ex.Message}");
                                }

                        }
                
            }


            Thread.Sleep(TimeSpan.FromMinutes(intervalMinutes)); 
        }
    }

    static async Task Menu(NpgsqlConnection connection)
    {
        Functions func = new Functions();
        Thread.Sleep(1000);
        string action = "";
                while (action != "5") 
                {
                    using (var cmd = new NpgsqlCommand("SELECT VERSION();", connection))
                        {
                            Console.WriteLine("Выберите действие:");
                            Console.WriteLine("1. Добавить данные");
                            Console.WriteLine("2. Обновить данные");
                            Console.WriteLine("3. Фильтровать данные");
                            Console.WriteLine("4. Показать таблицу");
                            Console.WriteLine("5. Выйти");

                            action = Console.ReadLine();

                            
                            if (action == "1" || action == "2" || action == "3" || action == "4")
                            {
                                break;
                            }
                            else if (action == "5")
                            {
                                Environment.Exit(0);
                            }
                            else
                            {
                                Console.WriteLine("Некорректный выбор. Пожалуйста, выберите действие из списка.");
                            }
                        }
                }

        Console.WriteLine("Выберите таблицу:");
        Console.WriteLine("1. Начинки");
        Console.WriteLine("2. Коржи");
        Console.WriteLine("3. Покупатели");
        Console.WriteLine("4. Заказы");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
           if (action == "1")
            {
                await Task.WhenAll(func.AddData("Filling", connection), func.ShowData("Filling", connection), Menu(connection));
            }
            if (action == "2")
            {
                await Task.WhenAll(func.UpdateData("Filling", connection), func.ShowData("Filling", connection), Menu(connection));
            }
            if (action == "3")
            {
                await Task.WhenAll(func.FilterData("Filling", connection), Menu(connection));
            }
            if (action == "4")
            {
                await Task.WhenAll(func.ShowData("Filling", connection), Menu(connection));
            }
        }
        else if (choice == "2")
        { 
           if (action == "1")
            {
                await Task.WhenAll(func.AddData("Cakes", connection), func.ShowData("Cakes", connection), Menu(connection));
            }
            if (action == "2")
            {
                await Task.WhenAll(func.UpdateData("Cakes", connection), func.ShowData("Cakes", connection), Menu(connection));
            }
            if (action == "3")
            {
                await Task.WhenAll(func.FilterData("Cakes", connection), Menu(connection));
            }
            if (action == "4")
            {
                await Task.WhenAll(func.ShowData("Cakes", connection), Menu(connection));
            }
        }
        else if (choice == "3")
        {
            if (action == "1")
            {
                await Task.WhenAll(func.AddData("Buyers", connection), func.ShowData("Buyers", connection), Menu(connection));
            }
            if (action == "2")
            {
                await Task.WhenAll(func.UpdateData("Buyers", connection), func.ShowData("Buyers", connection), Menu(connection));
            }
            if (action == "3")
            {
                //await func.FilterData("Buyers", connection);
                await Task.WhenAll(func.ShowData("Buyers", connection), func.FilterData("Buyers", connection), Menu(connection));
            }
            if (action == "4")
            {
                await Task.WhenAll(func.ShowData("Buyers", connection), Menu(connection));
            }
        }
        else if (choice == "4")
        {
            if (action == "1")
            {
                //await Task.WhenAll(func.AddData("Orders", connection), func.ShowData("Orders", connection), Menu(connection));
                await func.AddData("Orders", connection);
                await func.ShowData("Orders", connection);
                await Menu(connection);           
            }
            if (action == "2")
            {
                await Task.WhenAll(func.UpdateData("Orders", connection), func.ShowData("Orders", connection), Menu(connection));
            }
            if (action == "3")
            {
                await Task.WhenAll(func.ShowData("Orders", connection), func.FilterData("Orders", connection), Menu(connection));
            }
            if (action == "4")
            {
                await Task.WhenAll(func.ShowData("Orders", connection), Menu(connection));
            }
        }
    }

}

public class Functions
{
    public async Task AddData(string tableName, NpgsqlConnection connection)
    {
    if (tableName == "Buyers")
    {
        Console.Write($"Введите имя клиента для добавления в таблицу {tableName}: ");
        var name = Console.ReadLine();
        Console.Write($"Введите вес торта для добавления в таблицу {tableName}: ");
        int.TryParse(Console.ReadLine(), out int weight_cake);
        using (var instruction = new NpgsqlCommand($"INSERT INTO {tableName} (\"name\", \"weight_cake\") VALUES (@name, @weight_cake)", connection)) 
        {
            instruction.Parameters.AddWithValue("name", name);
            instruction.Parameters.AddWithValue("weight_cake", weight_cake);
            await instruction.ExecuteNonQueryAsync();
        }
        Console.WriteLine($"Данные успешно добавлены в таблицу {tableName}.");
    }
    else if (tableName == "Orders")
    {
        Console.Write($"Введите номер заказа для добавления в таблицу {tableName}: ");
        var order_number = Console.ReadLine();
        Console.Write($"Введите ID начинки для добавления в таблицу {tableName}: ");
        int.TryParse(Console.ReadLine(), out int filling_id);
        Console.Write($"Введите ID коржа для добавления в таблицу {tableName}: ");
        int.TryParse(Console.ReadLine(), out int cake_id);
        Console.Write($"Введите ID покупателя для добавления в таблицу {tableName}: ");
        int.TryParse(Console.ReadLine(), out int buyer_id);
        
        using (var instruction = new NpgsqlCommand($"INSERT INTO {tableName} (\"order_number\", \"filling_id\", \"cake_id\", \"buyer_id\") VALUES (@order_number, @filling_id, @cake_id, @buyer_id)", connection)) {
            instruction.Parameters.AddWithValue("order_number", order_number);
            instruction.Parameters.AddWithValue("filling_id", filling_id);
            instruction.Parameters.AddWithValue("cake_id", cake_id);
            instruction.Parameters.AddWithValue("buyer_id", buyer_id);
            
            await instruction.ExecuteNonQueryAsync();
            }
        Console.WriteLine($"Данные успешно добавлены в таблицу {tableName}.");
    }
    else
    {
        Console.Write($"Введите значение для добавления в таблицу {tableName}: ");
        string name = Console.ReadLine();
        
        using var instruction = new NpgsqlCommand($"INSERT INTO {tableName} (\"name\") VALUES (@name)", connection);
        instruction.Parameters.AddWithValue("name", name);
        
        await instruction.ExecuteNonQueryAsync();
        Console.WriteLine($"Данные успешно добавлены в таблицу {tableName}.");
    }
}

    public async Task UpdateData(string tableName, NpgsqlConnection connection)
    {   
        Thread.Sleep(1000);
        if (tableName == "Buyers")
        {
            ShowData("Buyers", connection);
            Thread.Sleep(500);
            object old_value = 0;
            object new_value = 0;
            Console.WriteLine($"Выберите столбец из таблицы {tableName}: ");
            Console.WriteLine("1. имя клиента");
            Console.WriteLine("2. вес торта");
            var column = Console.ReadLine();
            
            string columnName = (column == "1") ? "name" : "weight_cake";

            if (column == "1")
            {
                Console.WriteLine("Какое значение изменить? ");
                old_value = Console.ReadLine();
                Console.WriteLine("Какое новое значение? ");
                new_value = Console.ReadLine();
            }
            else if (column == "2")
            {
                Console.WriteLine("Какое значение изменить? ");
                int.TryParse(Console.ReadLine(), out int oldIntValue);
                old_value = oldIntValue;
                Console.WriteLine("Какое новое значение? ");
                int.TryParse(Console.ReadLine(), out int newIntValue);
                new_value = newIntValue;
            }
    
            using var instruction = new NpgsqlCommand($"UPDATE {tableName} SET {columnName} = @newValue WHERE {columnName} = @oldValue", connection);
            instruction.Parameters.AddWithValue("newValue", new_value);
            instruction.Parameters.AddWithValue("oldValue", old_value);

            await instruction.ExecuteNonQueryAsync();

            Console.WriteLine($"Данные успешно обновлены в таблице {tableName}.");
        }
        else if (tableName == "Orders") {
            
            ShowData("Orders", connection);
            Thread.Sleep(500);
            object new_value = 0;
            Console.WriteLine($"Выберите столбец из таблицы {tableName}: ");
            Console.WriteLine("1. Номер заказа");
            Console.WriteLine("2. ID Начинки");
            Console.WriteLine("3. ID Коржа");
            Console.WriteLine("4. ID Покупателя");
            var column = Console.ReadLine();
            string columnName = "";
            if (column == "1") {
                columnName = "order_number";
            }
            else if (column == "2") {
                columnName = "filling_id";
            }
            else if (column == "3") {
                columnName = "cake_id";
            }
            else if (column == "4") {
                columnName = "buyer_id";
            }

            Console.Write("Какой номер проекта? ");
            var Order_number = Console.ReadLine();

        if (column == "1")
        {
            Console.WriteLine("Какое новое значение? ");
            new_value = Console.ReadLine();
        }
        else if (column == "2" || column == "3" || column == "4")
        {
            Console.WriteLine("Какое новое значение? ");
            int.TryParse(Console.ReadLine(), out int newint_value);
                new_value = newint_value;
        }

            using var instruction = new NpgsqlCommand($"UPDATE {tableName} SET {columnName} = @newValue WHERE order_number = @Order_number", connection);
            instruction.Parameters.AddWithValue("newValue", new_value);
            instruction.Parameters.AddWithValue("Order_number", Order_number);

            await instruction.ExecuteNonQueryAsync();

            Console.WriteLine($"Данные успешно обновлены в таблице {tableName}.");
        }
        else {
            ShowData(tableName, connection);
            Console.WriteLine("Какое значение изменить? ");
            var old_value = Console.ReadLine();
            Console.WriteLine("Какое новое значение? ");
            var new_value = Console.ReadLine();
    
            using var instruction = new NpgsqlCommand($"UPDATE {tableName} SET name = @newValue WHERE name = @oldValue", connection);
            instruction.Parameters.AddWithValue("newValue", new_value);
            instruction.Parameters.AddWithValue("oldValue", old_value);
            await instruction.ExecuteNonQueryAsync();

            Console.WriteLine($"Данные успешно обновлены в таблице {tableName}.");
        }
    }

    public async Task FilterData(string tableName, NpgsqlConnection connection)
    {
        Thread.Sleep(1000);
        if (tableName == "Buyers")
        {
        Thread.Sleep(500);
        Console.WriteLine("По какому параметру настроить фильтрацию? ");
        Console.WriteLine("1. имя клиента");
        Console.WriteLine("2. вес торта");
        var column = Console.ReadLine();
        object filterValue = 0;
    
        string columnName = (column == "1") ? "name" : "weight_cake";
        if (column == "1")
        {
            Console.WriteLine($"Введите значение для фильтрации по столбцу {columnName}:");
            filterValue = Console.ReadLine();
        }
        else if (column == "2")
        {
            Console.WriteLine($"Введите значение для фильтрации по столбцу {columnName}:");
            int.TryParse(Console.ReadLine(), out int secondFilterValueInt);
            filterValue = secondFilterValueInt;
        }

        await using var instruction = new NpgsqlCommand($"SELECT * FROM {tableName} WHERE {columnName} = @filterValue", connection);
        instruction.Parameters.AddWithValue("filterValue", filterValue);

        Console.WriteLine("Хотите установить значение фильтрации для другого столбца? (да/нет)");
        var answer = Console.ReadLine();
        
        if (string.Equals(answer, "да", StringComparison.CurrentCultureIgnoreCase))
        {
            var secondColumnName = (columnName == "name") ? "weight_cake" : "name";
            object secondFilterValue = 0;
            
            if (secondColumnName == "name")
            {
                Console.WriteLine($"Введите значение для фильтрации по столбцу {secondColumnName}:");
                secondFilterValue = Console.ReadLine();
            }
            else if (secondColumnName == "weight_cake")
            {
                Console.WriteLine($"Введите значение для фильтрации по столбцу {secondColumnName}:");
                int.TryParse(Console.ReadLine(), out int secondFilterValueInt);
                secondFilterValue = secondFilterValueInt;
            }
            
            instruction.CommandText += $" AND {secondColumnName} = @secondFilterValue";
            instruction.Parameters.AddWithValue("secondFilterValue", secondFilterValue);
        }
        await using var reader = await instruction.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                    Console.Write($"{reader[i]} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        
    }
    else if (tableName == "Orders")
    {
        Thread.Sleep(500);
        Console.WriteLine($"Выберите столбец из таблицы {tableName}: ");
        Console.WriteLine("1. Номер заказа");
        Console.WriteLine("2. ID начинки");
        Console.WriteLine("3. ID коржа");
        Console.WriteLine("4. ID покупателя");
        var column = Console.ReadLine();
        string columnName = "";
        if (column == "1") {
            columnName = "Order_number";
        }
        else if (column == "2") {
            columnName = "Filling_id";
        }
        else if (column == "3") {
            columnName = "Cake_id";
        }
        else if (column == "4") {
            columnName = "Buyer_id";
        }
        object columnValue = 0;

         if (columnName == "Order_number")
            {
                Console.WriteLine($"Введите значение для фильтрации по столбцу {columnName}:");
                columnValue = Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"Введите значение для фильтрации по столбцу {columnName}:");
                int.TryParse(Console.ReadLine(), out int columnValueInt);
                columnValue = columnValueInt;
            }

        using var instruction = new NpgsqlCommand($"SELECT * FROM {tableName} WHERE {columnName} = @columnValue", connection);
        instruction.Parameters.AddWithValue("columnValue", columnValue);
        
        Console.WriteLine("Хотите установить значение фильтрации для другого столбца? (да/нет)");
        var answer = Console.ReadLine();
        
        if (answer.ToLower() == "да")
        {
            Console.WriteLine($"Выберите столбец из таблицы {tableName}: ");
            Console.WriteLine("1. Order number");
            Console.WriteLine("2. ID Filling");
            Console.WriteLine("3. ID Cake");
            Console.WriteLine("4. ID buyer");
            var secondcolumn = Console.ReadLine();
            string secondColumnName = "";
            if (secondcolumn == "1") {
                secondColumnName = "Order_number";
            }
            else if (secondcolumn == "2") {
                secondColumnName = "Filling_id";
            }
            else if (secondcolumn == "3") {
                secondColumnName = "Cake_id";
            }
            else if (secondcolumn == "4") {
                secondColumnName = "Buyer_id";
            }
            object secondFilterValue = 0;
            
            if (secondColumnName == "Order_number")
            {
                Console.WriteLine($"Введите значение для фильтрации по столбцу {secondColumnName}:");
                secondFilterValue = Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"Введите значение для фильтрации по столбцу {secondColumnName}:");
                int.TryParse(Console.ReadLine(), out int secondFilterValueInt);
                secondFilterValue = secondFilterValueInt;
            }
            
            instruction.CommandText += $" AND {secondColumnName} = @secondFilterValue";
            instruction.Parameters.AddWithValue("secondFilterValue", secondFilterValue);
        }
        await using var reader = await instruction.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                    Console.Write($"{reader[i]} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    else {
        ShowData(tableName, connection);
        Thread.Sleep(500);
        Console.WriteLine($"Введите значение для фильтрации по столбцу name:");
        var filterValue = Console.ReadLine();
    
        using var instruction = new NpgsqlCommand($"SELECT * FROM (SELECT * FROM {tableName} WHERE name = @filterValue) AS filteredResults", connection);
        instruction.Parameters.AddWithValue("filterValue", filterValue);
    
        await using var reader = await instruction.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.Write($"{reader[i]} ");
            }
            Console.WriteLine();
        }
    }
}

    public async Task ShowData(string tableName, NpgsqlConnection connection)
    {
        Thread.Sleep(500);
        Console.WriteLine($"Вывод таблицы {tableName}.");

        await using var command = new NpgsqlCommand($"SELECT * FROM {tableName}", connection);
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                    Console.Write($"{reader[i]} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
        
}

