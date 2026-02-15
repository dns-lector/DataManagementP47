using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManagementP47.Ado.Dal
{
    internal class DataAccessor(SqlConnection connection)
    {
        private SqlConnection _connection = connection;

        public int GetProductCount() => Convert.ToInt32( QueryScalar("SELECT COUNT(*) FROM Products") );

        public int GetEmployeeCount()
        {
             return Convert.ToInt32( QueryScalar("SELECT COUNT(*) FROM Employees") );
        }

        public DateTime GetFirstSaleMoment() => Convert.ToDateTime( QueryScalar("SELECT MIN(Moment) FROM Sales") );

        public Double GetAvgSale() => Convert.ToDouble( 
            QueryScalar("SELECT AVG(S.Quantity * P.Price) FROM Sales S JOIN Products P ON S.ProductId = P.Id") );

        private object QueryScalar(String sql)
        {
            try
            {
                using SqlCommand command = new(sql, _connection);
                return command.ExecuteScalar();
                /* ExecuteScalar - повертає один результат, 
                 * якщо у запиті є множинний результат, береться лівий верхній кут таблиці
                 * Тип повернення - object - може повертати довільний тип даних
                 */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new InvalidOperationException();
            }
        }
    }
}
/* DAL - Data Access Layer - Шар доступу до даних
 * містить об'єкт або сукупність об'єктів, які 
 * реалізують логіку виконання запитів та повертають
 * їх результати за ORM
 */