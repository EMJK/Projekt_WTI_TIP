using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace VoipServer
{
    class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        private NpgsqlConnection CreateConnection()
        {
            var con = new NpgsqlConnection(_connectionString);
            con.Open();
            return con;
        }

        public DataTable GetDataFromDB(string command, string paramName, string paramValue)
        {
            using (var connection = CreateConnection())
            using (var sqlc = new NpgsqlCommand(command))// NpgsqlCommand instance
            {
                DataTable dt = new DataTable(); // DataTable instance named dt
                NpgsqlParameter par = new NpgsqlParameter(paramName, paramValue);
                sqlc.Parameters.Add(par);

                try
                {
                    sqlc.Connection = connection; // connection to DB
                    using (var dr = sqlc.ExecuteReader())// query execution and creation of dr pointer
                    {
                        dt.Load(dr); //load data to dataTable object
                    }
                    return dt; // return data
                }
                catch
                {
                    return null;
                }
            }
        }

        public int WriteDataToDB(string command, string paramName, object paramValue)
        {
            using (var connection = CreateConnection())
            using (var sqlc = new NpgsqlCommand(command))// NpgsqlCommand instance to execute query
            {
                sqlc.Connection = connection; // connection to DB
                NpgsqlParameter par = new NpgsqlParameter(paramName, paramValue);
                sqlc.Parameters.Add(par);
                int result = sqlc.ExecuteNonQuery();
                return result;
            }
        }

        public int WriteDataToDB(string command, Dictionary<string, object> parameters)
        {
            using (var connection = CreateConnection())
            using (var sqlc = new NpgsqlCommand(command)) // NpgsqlCommand instance to execute query
            {
                sqlc.Connection = connection; // connection to DB
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    NpgsqlParameter par = new NpgsqlParameter(parameter.Key, parameter.Value);
                    sqlc.Parameters.Add(par);
                }
                int result = sqlc.ExecuteNonQuery();
                return result;
            }
        }
    }
}