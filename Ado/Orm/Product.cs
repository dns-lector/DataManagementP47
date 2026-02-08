using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataManagementP47.Ado.Orm
{
    internal class Product
    {
        public Guid   Id    { get; set; }
        public String Name  { get; set; } = null!;
        public double Price { get; set; }

        public static Product FromReader(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetGuid("Id"),
                Name = reader.GetString("Name"),
                Price = Convert.ToDouble( reader.GetDecimal("Price") )
            };
        }
    }
}
/*
 * CREATE TABLE Products(
    Id    UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name  NVARCHAR(256)    NOT NULL,
    Price MONEY            NOT NULL
);
 */