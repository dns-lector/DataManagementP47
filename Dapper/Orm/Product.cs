using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataManagementP47.Dapper.Orm
{
    internal class Product
    {
        public Guid   Id    { get; set; }
        public String Name  { get; set; } = null!;
        public double Price { get; set; }

        public override string ToString()
        {
            String first3 = Id.ToString()[..3];
            String last3 = Id.ToString()[^3..];
            return $"{first3}..{last3}  {Name}  ₴{Price:F2}";
        }
    }
}