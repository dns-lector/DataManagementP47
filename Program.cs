using DataManagementP47.Ado;
using DataManagementP47.Dapper;
using DataManagementP47.EF;

Console.OutputEncoding = System.Text.Encoding.Unicode;
Console.WriteLine("Data Management");

//new Ado().Run();
//new DapperDemo().Run();
new EfDemo().Run();
