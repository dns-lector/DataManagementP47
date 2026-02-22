using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataManagementP47.Dapper.Orm
{
    internal class Employee
    {
        public Guid     Id        { get; set; }
        public Guid     FirmId    { get; set; }
        public String   Name      { get; set; } = null!;
        public DateTime Birthdate { get; set; }
    }
}