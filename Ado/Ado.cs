using Microsoft.Data.SqlClient;

namespace DataManagementP47.Ado
{
    public class Ado
    {
        String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\samoylenko_d\source\repos\DataManagementP47\Database1.mdf;Integrated Security=True";
        // String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lector\source\repos\DataManagementP47\Database1.mdf;Integrated Security=True";
        private SqlConnection? connection;

        public void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.WriteLine("ADO.NET");
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.WriteLine("1 - Перевірити підключення");
                Console.WriteLine("2 - Створити таблиці БД");
                Console.WriteLine("0 - Вихід");
                keyInfo = Console.ReadKey();
                Console.WriteLine();

                switch (keyInfo.KeyChar)
                {
                    case '1': OpenConnection(); break;
                    case '2': CreateTables(); break;
                    case '0': break;
                    default: Console.WriteLine("Неправильний вибір"); break;
                }
            } while (keyInfo.KeyChar != '0');
            

        }

        private void CreateTables()
        {
            if(connection is null)
            {
                Console.WriteLine("Підключення не встановлене, оберіть спочатку п.1");
                return;
            }

            // Другий етап - виконання команд (SQL-інструкцій)
            // Передача команд до СУБД та відповіді назад покладається
            // на SqlCommand
            // String sql = @"CREATE TABLE Firms(
            //    Id   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            //    Name NVARCHAR(64)     NOT NULL )";
            // команди є некерованими ресурсами, бажано вживати 
            // оператор автоматичного закриття - using//
            String sql = File.ReadAllText(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "sql/Tables.sql"
            ));
            // Команди також можуть виконуватись з файлу, у т.ч.
            // кілька команд, розділених ";"

            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();   // виконання без повернення результату
                Console.WriteLine("Create Tables OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create Tables fail: {ex.Message}");
                Console.WriteLine(sql);
            }
        }
    
        private void OpenConnection()
        {
            // початок роботи з БД - підключення
            // традиція - рядок підключення - всі дані зібрані
            // до одного виразу

            // управління підключенням забезпечує SqlConnection (Microsoft.Data.SqlClient)
            connection = new(connectionString);
            // однак, створення об'єкту не створює підключення (на відміну від
            //  багатьох аналогічних технологій)
            // Для активації підключення потрібно виконати команду Open
            try
            {
                connection.Open();
                Console.WriteLine("Connection OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection fail: {ex.Message}");
            }
        }
    }
}
/* ADO.NET - базова технологія доступу до даних платформи .NET :
додаємо бібліотеку (NuGet) з групи Microsoft.Data під обрану для
проєкту СУБД. Зокрема, для MS SQL Server це
Microsoft.Data.SqlClient

[Firms]   [Employee]    [Sales]        [Products]
Id --\    Id   ---\     Id           /- Id
Name  \-- FirmId   \--- EmployeeId  /   Name
          Name          ProductId -/    Price
          Birthday      Quantity   
                        Moment     

 */