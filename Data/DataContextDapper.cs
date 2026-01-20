using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data

{
    class DataContextDapper
    {
        private readonly IConfiguration _config;
        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }   

        public IEnumerable<T> LoadData<T>(string sql)
        {
            using IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql) // explanation: Generic method to load a single record from the database
        {
            using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")); 
            return dbConnection.QuerySingle<T>(sql);
        } 

        public bool ExecuteSql(string sql) // explanation: Generic method to execute a SQL command (like INSERT, UPDATE, DELETE)
                 {
                      using IDbConnection dbConnection = new SqlConnection(
                         _config.GetConnectionString("DefaultConnection")); 
                     return (dbConnection.Execute(sql) > 0); // returns true if one or more rows were affected
                 } 

        public int ExecuteSqlWithRowCount(string sql, object parameters) // explanation: Generic method to execute a SQL command (like INSERT, UPDATE, DELETE)
                 {
                      using IDbConnection dbConnection = new SqlConnection(
                            _config.GetConnectionString("DefaultConnection")
                      ); 
                     return dbConnection.Execute(sql, parameters); // returns the number of rows affected 
                       
                    
                 }
        

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters) // explanation: Generic method to execute a SQL command (like INSERT, UPDATE, DELETE)
                 {
                     SqlCommand commandWithParams = new SqlCommand(sql);

                     foreach(SqlParameter parameter in parameters)
                     {
                        commandWithParams.Parameters.Add(parameter);
                     }
                     SqlConnection dbConnection = new SqlConnection(
                            _config.GetConnectionString("DefaultConnection")
                      ); 
                    dbConnection.Open();
                    commandWithParams.Connection = dbConnection;
                    int rowsAffected = commandWithParams.ExecuteNonQuery();
                    dbConnection.Close();
                    return (rowsAffected > 0);
                 }


            
    }
}