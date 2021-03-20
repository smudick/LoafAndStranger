using Dapper;
using LoafAndStranger.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoafAndStranger.DataAccess
{
    public class TopsRepository
    {
        const string ConnectionString = "Server=localhost;Database=LoafAndStranger;Trusted_Connection=True;";
        public IEnumerable<Top> GetAll()
        {
            using var db = new SqlConnection(ConnectionString);

            var sql = @"Select *
                        from Tops";
            return db.Query<Top>(sql);
        }

        public Top Add(int numberOfSeats)
        {
            using var db = new SqlConnection(ConnectionString);

            var sql = @"INSERT INTO [dbo].[Tops] ([NumberOfSeats])
                        OUTPUT inserted.*
                        VALUES (@numberOfSeats)";
            var top = db.QuerySingle<Top>(sql, new { numberOfSeats });

            return top;
        }
    }
}
