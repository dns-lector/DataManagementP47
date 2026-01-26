using Microsoft.Data.SqlClient;

namespace DataManagementP47.Ado
{
    public class Ado
    {
        public void Run()
        {
            Console.WriteLine("ADO.NET");
            // початок роботи з БД - підключення
            // традиція - рядок підключення - всі дані зібрані
            // до одного виразу
            // String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\samoylenko_d\source\repos\DataManagementP47\Database1.mdf;Integrated Security=True";
            String connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lector\source\repos\DataManagementP47\Database1.mdf;Integrated Security=True";
            
            // управління підключенням забезпечує SqlConnection (Microsoft.Data.SqlClient)
            SqlConnection connection = new(connectionString);
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

            // Другий етап - виконання команд (SQL-інструкцій)
            // Передача команд до СУБД та відповіді назад покладається
            // на SqlCommand
            String sql = @"CREATE TABLE Firms(
               Id   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
               Name NVARCHAR(64)     NOT NULL )";
            // команди є некерованими ресурсами, бажано вживати 
            // оператор автоматичного закриття - using//
            SqlCommand cmd;
            cmd = new();
            cmd.CommandText = sql;
            cmd.Connection = connection;
            try
            {                
                cmd.ExecuteNonQuery();   // виконання без повернення результату
                Console.WriteLine("Command OK");
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Command fail: {ex.Message}");
                Console.WriteLine(sql);
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