using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataManagementP47.Ado.Orm
{
    internal class Employee
    {
        public Guid     Id        { get; set; }
        public Guid     FirmId    { get; set; }
        public String   Name      { get; set; } = null!;
        public DateTime Birthdate { get; set; }

        public static Employee FromReader(SqlDataReader reader)
        {
            return new Employee
            {
                Id = reader.GetGuid("Id"),
                FirmId = reader.GetGuid("FirmId"),
                Name = reader.GetString("Name"),
                Birthdate = reader.GetDateTime("Birthdate")
            };
        }
    }
}
/* CREATE TABLE Employees(
    Id        UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    FirmId    UNIQUEIDENTIFIER NOT NULL,
    Name      NVARCHAR(100) NOT NULL,
    Birthdate DATETIME2        NOT NULL
);
 */