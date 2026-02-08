using DataManagementP47.Ado.Orm;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;

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
                Console.WriteLine("3 - Заповнити початкові дані");
                Console.WriteLine("4 - Створити дані 1000 продажів");
                Console.WriteLine("5 - Вивести дані про фірми");
                Console.WriteLine("0 - Вихід");
                keyInfo = Console.ReadKey();
                Console.WriteLine();

                switch (keyInfo.KeyChar)
                {
                    case '1': OpenConnection(); break;
                    case '2': CreateTables(); break;
                    case '3': InsertData(); break;
                    case '4': GenerateSales(); break;
                    case '5': ShowFirms(); break;
                    case '0': break;
                    default: Console.WriteLine("Неправильний вибір"); break;
                }
            } while (keyInfo.KeyChar != '0');
        }

        // 4 
        private void GenerateSales()
        {
            if (connection is null)
            {
                Console.WriteLine("Підключення не встановлене, оберіть спочатку п.1");
                return;
            }
            List<Product> products = [];
            {
                using SqlCommand cmd1 = new("SELECT * FROM Products", connection);
                using SqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    products.Add(Product.FromReader(reader1));
                }
            }

            List<Employee> employees = [];
            {
                using SqlCommand cmd2 = new("SELECT * FROM Employees", connection);
                using SqlDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    employees.Add(Employee.FromReader(reader2));
                }
            }

            Random random = new();

            String sql = "INSERT INTO Sales(Id, EmployeeId, ProductId, Quantity, Moment) " +
                             "VALUES( NEWID(), @EmployeeId, @ProductId, @Quantity, @Moment )";
            using SqlCommand cmd = new(sql, connection);
            SqlParameter employeeParam = new() { ParameterName = "@EmployeeId", DbType = DbType.Guid };
            cmd.Parameters.Add(employeeParam);

            SqlParameter productParam = new() { ParameterName = "@ProductId", DbType = DbType.Guid };
            cmd.Parameters.Add(productParam);

            SqlParameter quantityParam = new() { ParameterName = "@Quantity", DbType = DbType.Int32 };
            cmd.Parameters.Add(quantityParam);

            // якщо тип даних дозволяє різні розміри, то необхідно зазначити Size
            // (для DateTime2 це може бути 6, 7 або 8 байт в залежності від точності) - деталі у конструкторі БД
            SqlParameter momentParam = new() { ParameterName = "@Moment", DbType = DbType.DateTime2, Size = 7 };
            cmd.Parameters.Add(momentParam);

            cmd.Prepare();  // Підготовлений запит - який можна виконувати декілька разів з різними параметрами

            for (int i = 0; i < 1000; i++)
            {
                Employee employee = employees[random.Next(employees.Count)];
                Product product = products[random.Next(products.Count)];
                // випадкова кількість з урахуванням, що чим дешевший товар, тим більша кількість може бути
                int quantity = 1 + random.Next((int) (1000 / product.Price / 3));
                // випадковий момент часу - у діапазоні останніх 10 років (315360000 секунд)
                DateTime moment = DateTime.Now.AddSeconds(-random.Next(315360000));

                cmd.Parameters[0].Value = employee.Id;
                cmd.Parameters[1].Value = product.Id;
                cmd.Parameters[2].Value = quantity;
                cmd.Parameters[3].Value = moment;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Помилка: " + ex.Message);
                    return;
                }
            }
            Console.WriteLine("Команди виконано успішно");
            /* Д.З. Доповнити метод додавання (генерування) продажів:
             * наприкінці його роботи додати відомості про поточну кількість продажів:
             * - Команди виконано успішно, загальна кількість продажів: 3000
             */
        }

        private void ParametersInQueries()
        {
            /* Параметризовані запити
            Передісторія: 
            1) SQL-ін'єкції - спосіб включити до даних коди так, щоб вони виконались у запиті
                SELECT * FROM Users WHERE login='{str}'
                login> user
                SELECT * FROM Users WHERE login='user' 
                login> user' OR '1'='1
                SELECT * FROM Users WHERE login='user' OR '1'='1' 
                login> user' OR (DROP TABLE Users)='1
                SELECT * FROM Users WHERE login='user' OR (DROP TABLE Users)='1' 

            2) Локалізація та національні формати
                UPDATE Products SET Price={x} WHERE Id='1234124'
                x = 99.50
                з урахуванням української локалізації число формується через кому 99.50 -> 99,50
                UPDATE Products SET Price=99,50 WHERE Id='1234124'  -- помилка синтаксису
             
            - Неможна "змішувати" код та дані, особливо якщо походження даних не є надійним (введеня
               користувача, прийом з мережі, витяг з файлу тощо)
            - Слід вживати заходи з відокремлення кодів та даних
               SELECT * FROM Users WHERE login='UTF_DECODE({str})'

            !! Параметризовані запити - інструмент такого відокремлення: у запит включаються
                плейсхолдери - іменовані параметри, а окремо подаються їх значення, зазвичай,
                із зазначенням типу даних. Синтаксис плейсхолдерів сильно залежить від технологій
                (мови програмування, СУБД, драйверів тощо)
             */
            if (connection is null)
            {
                Console.WriteLine("Підключення не встановлене, оберіть спочатку п.1");
                return;
            }
            String sql = "SELECT * FROM Firms WHERE Name = @name";   // @name - параметр або іменований плейсхолдер
                                                                     // !! лапки не додаємо (Name = '@name' - неправильно)
            using SqlCommand cmd = new(sql, connection);
            SqlParameter nameParameter = new()
            {
                ParameterName = "@name",                             // як у плейсхолдера
                SqlDbType = SqlDbType.NVarChar,                      // із зазначенням типу даних
                Value = "EcoResale JSC"                              // сюди безпечно підставляти введені дані
            };                                                      
            cmd.Parameters.Add(nameParameter);                       // Всі параметри додаються до колекції команди: cmd.Parameters
            using SqlDataReader reader = cmd.ExecuteReader();        // Після додавання усіх параметрів
            if (reader.Read())                                       // запити виконуються і обробляються
            {                                                        // таким же чином, як і без параметрів 
                Console.WriteLine("{0}  {1}",
                    reader.GetGuid("Id"),
                    reader.GetString("Name"));
            }
            else
            {
                Console.WriteLine("Не знайдено");
            }
        }

        // 5
        private void ShowFirms()
        {
            if (connection is null)
            {
                Console.WriteLine("Підключення не встановлене, оберіть спочатку п.1");
                return;
            }
            // Команди, що повертають результат. Вибірки
            String sql = "SELECT * FROM Firms";
            using SqlCommand cmd = new(sql, connection);
            // Передачу резульатів забезпечує SqlDataReader
            using SqlDataReader reader = cmd.ExecuteReader();
            // "Гнучкість" до різної кількості колонок забезпечує індексація
            // reader["Id"], reader["Name"]
            // або спеціальні геттери
            // reader.GetGuid("Id"), reader.GetString("Name")
            // Геттери більш рекомендовані через типізацію
            //    while (reader.Read())   // ітерація генератора - одержання одного рядка таблиці
            //    {
            //        Console.WriteLine("{0}  {1}", 
            //            reader.GetGuid("Id"), 
            //            reader.GetString("Name"));
            //    }
            // ORM - Object Relation Mapping - Відображення даних та їх зв'язків
            //  на об'єкти та їх зв'язки. Використовується коли формат даних не
            //  є сумісним з мовою програмування (БД, JSON, XML, Excel ...)
            // У найпростішому випадку - це оголошення класів та утворення колекції
            //  їх об'єктів
            List<Firm> firms = [];
            while (reader.Read())
            {
                firms.Add(Firm.FromReader(reader));   // Add(new Firm(reader))
            }

            // Від'єднаний режим -- працюємо з колекціями, попередньо завантаженими з БД
            foreach (Firm firm in firms)
            {
                Console.WriteLine(firm);
            }
        }

        // 3
        private void InsertData()
        {
            /* Сідування (від англ. seed - зерно) - внесення початкових
             * даних до системи, у т.ч. БД
             */
            if (connection is null)
            {
                Console.WriteLine("Підключення не встановлене, оберіть спочатку п.1");
                return;
            }
            String sql = File.ReadAllText(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "sql/InsertData.sql"
            ));

            using SqlCommand cmd = new(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();   // виконання без повернення результату
                Console.WriteLine("Insert Data OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Insert Data fail: {ex.Message}");
                Console.WriteLine(sql);
            }
        }

        // 2
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
    
        // 1
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

Д.З. Наповнити БД "Академія" тестовими даними 
Додати скріншоти з наповненими таблицями
 */