using Dapper;
using DataManagementP47.Dapper.Orm;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManagementP47.Dapper
{
    internal class DapperAccessor(SqlConnection connection)
    {
        private readonly SqlConnection _connection = connection;

        public List<Product> GetProducts() => [..
            // Query (Dapper extension) - формує генератор колекції (IEnumerable) заданого типу (Product)
            _connection.Query<Product>("SELECT * FROM Products")
        ];

        public Product RandomProduct() =>
            // QueryFirst - формує один об'єкт (не колекцію) за результатом запиту
            _connection.QueryFirst<Product>("SELECT TOP 1 * FROM Products ORDER BY NEWID()");

        public Firm RandomFirm() =>
            // QueryFirst - формує один об'єкт (не колекцію) за результатом запиту
            _connection.QueryFirst<Firm>("SELECT TOP 1 * FROM Firms ORDER BY NEWID()");

        public List<Product> SearchProducts(String fragment) => [..
            _connection.Query<Product>("SELECT P.* FROM Products P WHERE  P.Name LIKE @fragment",
                new {fragment = $"%{fragment}%"})
        ];

    }
}
