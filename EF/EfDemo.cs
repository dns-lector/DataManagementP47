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
            Console.WriteLine(context.Products.Count());
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