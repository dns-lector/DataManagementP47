using Dapper;
using DataManagementP47.Dapper.Orm;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DataManagementP47.Dapper
{
    internal class DapperDemo
    {
        private SqlConnection? connection;

        public void Run()
        {
            Console.WriteLine("Dapper demo");
            OpenConnection();
            DapperAccessor accessor = new(connection!);

            // best practice - формувати один блок для виведення замість багаторазового виведення малих блоків
            Console.WriteLine(String.Join('\n', accessor.SearchProducts("USB")));

            Console.WriteLine($"Випадковий товар у БД: {accessor.RandomProduct()}");
            Console.WriteLine($"Випадкова фірма у БД: {accessor.RandomFirm()}");
        }

        private void OpenConnection()
        {
            // копія з Ado.cs
            String connectionString;
            try
            {
                var config = JsonSerializer.Deserialize<JsonElement>(
                    File.ReadAllText(
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "appsettings.json"
                        )
                    )
                );
                connectionString = config.GetProperty("ConnectionStrings").GetProperty("AdoDB").GetString()!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Do you check appsettings.json?\n");
                return;
            }
            connection = new(connectionString);
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
/* Dapper являє собою micro-ORM. https://www.learndapper.com/
 * Головна задача: перетворювати результати SQL-запитів на об'єкти та їх колекції
 * 
 * Встановлення: NuGet: Dapper
 * Використання: методи розширення діють для Connection, встановлення підключення
 *  слід здійснити до початку використання Dapper
 * ! методи розширення підключаються тільки директивою using Dapper; 
 * 
 * Д.З. Виконати попереднє Д.З. засобами Dapper
// попереднє Д.З. Вивести статистичні дані (п. 6 меню):
// - кількість записів по всіх таблицях (у т.ч. фірми та продажі)
// - детальніше про продажі: дата першого та останнього продажу,
//    мінімальну, максимальну, середню суму чека (продажу)
//    максимальну кількість товарів в одному чеку
// - детальніше про фірми: мінімальна та максимальна кількість співробітників
 */
