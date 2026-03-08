using DataManagementP47.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManagementP47.EF
{
    internal class EfDemo
    {
        public void Run()
        {
            EfContext context = new();
            // Запити EF переслідують головну ідею - однаковий синтаксис незалежно від СУБД
            // MS SQL (T-SQL):  SELECT CURRENT_TIMESTAMP AS [дата-час]           | Неможна побудувати
            // MySQL  (P-SQL):  SELECT CURRENT_TIMESTAMP AS `дата-час`           | єдиний SQL, валідний
            // Oracle (PL-SQL): SELECT CURRENT_TIMESTAMP AS "дата-час" FROM DUAL | для усіх СУБД ...
            // ... проте можна приховати деталі у різних драйверах БД, залишивши однаковий LINQ

            // !! не плутати з LINQ-to-Collection, який призначений для роботи з ітерованими даними
            String[] arr = ["str 1", "str 2", "str 3", "str 4",];
            var res = from s in arr where s.StartsWith("str") select s;   // тип - IEnumerable

            // форма 1: from... застаріла, можна побачити у старих документаціях
            var query1 = from p in context.Products where p.Name.StartsWith("HDD") select p;    // тип - IQueryable
            // LINQ-to-SQL - різновид LINQ, який
            // будує SQL-запит для одержання відповідної інформації з БД
            // !! будує але не виконує, виконання запускається:
            //    - використанням агрегаційної функції (на кшталт .Count())
            //    - циклом-ітератором
            Console.WriteLine(query1.CreateDbCommand().CommandText); 
            Console.WriteLine();
            foreach (var product in query1)
            {
                Console.WriteLine(product);
            }
            Console.WriteLine($"Разом товарів: {query1.Count()}");
            Console.WriteLine("-----------------------------------------");

            // форма 2: Extension methods - методи розширення
            // повністю еквівалентна формі 1 (не дає переваг чи недоліків)
            var query2 = context.Products                // методи розширення базуються на лямбда-виразах
                .Where(p => p.Name.StartsWith("HDD"));   // предикат - різновид ф-цій, що повертають bool
            // Where - фільтр за умовою. !! але не всі інструменти є доступними для 
            // складання умов - тільки такі, що можуть бути переведені до SQL

            Console.WriteLine(query2.CreateDbCommand().CommandText);
            Console.WriteLine();
            foreach (var product in query2)
            {
                Console.WriteLine(product);
            }
            Console.WriteLine($"Разом товарів: {query2.Count()}");
            Console.WriteLine("-----------------------------------------");

            // Трансформація вибірки (.Select)
            var query3 = context.Products               
                .Where(p => p.Name.StartsWith("HDD"))
                .Select(p => new { p.Name, p.Price });   // як перетворення з Product до нового об'єкту
            foreach (var item in query3)
            {
                Console.WriteLine($"{item.Name} -- {item.Price}");
            }
            Console.WriteLine("-----------------------------------------");

            try
            {
                var query4 = context.Products                                  // Помилка - при трансформації
                    .Where(p => p.Name.StartsWith("HDD"))                      // до SQL EF не може "зобразити"
                    .Select(p => new { Name = HideCenter(p.Name), p.Price });  // внутр. метод HideCenter
                foreach (var item in query4)                                   // Причому виникає помилка не  
                {                                                              // при оголошенні, а при виконанні
                    Console.WriteLine($"{item.Name} -- {item.Price}");
                }
            }
            catch(InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);   // The client projection contains a reference to ...
            }
            Console.WriteLine("-----------------------------------------");

            var query5 = context.Products               // Коли неможливо побудувати SQL, але обробка
                .Where(p => p.Name.StartsWith("HDD"))   // все одно потрібна, тоді розділяємо:
                .AsEnumerable()                         // AsEnumerable - "границя" Queryable\Enumerable
                .Select(p => new {                      // Після цієї команди виконується SQL
                    Name = HideCenter(p.Name),          // і його результати обробляються як колекція
                    p.Price });                         // для якої немає обмежень щодо методів, які
            foreach (var item in query5)                // можуть використовуватись.
            {                                           // Але слід розуміти, що засобами SQL слід
                Console.WriteLine(                      // максимально обмежувати результати, оскільки
                    $"{item.Name} -- {item.Price}"      // саме вони задають початковий розмір множини
                );                                      // .AsEnumerable().Where(...) - вибір 114 з БД
            }                                           // з подальшим фільтром до 5 засобами С#
                                                        // .Where(...).AsEnumerable() - вибір 5 з БД
            Console.WriteLine("-----------------------------------------");

            Console.Write("Знайти перший товар за фрагментом (введіть): ");
            String fragment = Console.ReadLine()!;                // "м'який" пошук - з поверненням
                                                                  // значення за замовчанням (NULL)
            Product? prod = context.Products                      // .FirstOrDefault
                .FirstOrDefault(p => p.Name.Contains(fragment));  // "жорсткий" пошук .First
                                                                  // кидає виняток якщо дані не 
            Console.WriteLine(prod?.ToString() ?? "Не знайдено"); // знаходяться
                                                                  // .Find - шукає за ключем (Id)
            Console.WriteLine("-----------------------------------------");
            // Перетворити запит prod таким чином, щоб повертались лише назва та ціна
            /* Д.З. Реалізувати пошук співробітників:
             * на старті програма видає меню
             * 1 - пошук за іменем
             * 2 - пошук за віком
             * Далі пропонується ввести фрагмент імені або вік для пошуку
             * Далі здійснюється пошук та виводяться усі збіги
             * Якщо результатів пошуку немає, виводити відповідне повідомлення
             */
        }

        private String HideCenter(String input)
        {
            if (input.Length < 5) return "****";
            else return $"{input[..2]}**{input[^2..]}";
        }
    }
}
/* Entity Framework .NET Core
 * Універсальний інструмент для роботи з різними СУБД єдиним способом.
 * Замість SQL, який має різні діалекти для різних СУБД, EF вживає LINQ розширення
 * 
 * Встановлення: пакети NuGet
 * - Microsoft.EntityFrameworkCore - вибираємо версію з першою цифрою як у платформи .NET програми (10)
 *    ядро фреймворка, основні класи, типи, розширення
 * - Microsoft.EntityFrameworkCore.SqlServer - драйвери для MS SQL - реалізація базових типів
 *    під MSSQL Server
 * - Microsoft.EntityFrameworkCore.Tools - інструменти командного рядка, що спрощують перехідні процеси
 * 
 * Режими роботи
 * - Data First - початок роботи за умови, що існує раніше створена БД і необхідно її використовувати
 * - Code First - коли БД немає і можна почати розробку так, щоб БД була створенна у відповідності до коду
 * 
 * Data First - у проєкті є рядок підключення до БД, неохідно її опрацювати
 * - Відкриваємо РМС (Project Management Console) Tools - NuGet - PM Console
 * - Вводимо команду Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;...' Microsoft.EntityFrameworkCore.SqlServer
 * (замість ... підставляємо реальний рядок підключення)
 * (деталі: https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=vs)
 * - В результаті утворюються файли-сутності (класи) та контекст (клас нащадок DbContext)
 * 
 * Code First
 * Навпаки, ми самі створюємо класи-сутності та контекст (або копіюємо їх з Scaffold)
 * Підготувавши контекст створюємо міграції - спосіб внесення змін до БД схожий з комітами
 * 
 * PM> Add-Migration Initial
 * В результаті додається директорія Migrations
 * Сама міграція - це лише код, який дозволяє відновити стан БД або поверненням до 
 * попереднього стану, або для ініціалізації нової БД. Запуск міграцій - команда
 * PM> Update-Database
 * Після чого у БД має зафіксуватись відображення контексту
 * Для підключення до БД:
 * - копіюємо назву сервера (localdb)\MSSQLLocalDB
 * - Tools - Connect to database - (якщо запитує вибираємо MS SQL SERVER) - вставляємо назву сервера і вибираємо БД (EfDmP47)
 * 
 * Будь-які зміни у контексті даних вимагають створення та впровадження міграції
 * Наприклад, після того, як було додано дані сідування, знов створюємо міграцію
 * PM> Add-Migration Seeding
 * PM> Update-Database
 */