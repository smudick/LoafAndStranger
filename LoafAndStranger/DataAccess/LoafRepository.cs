using LoafAndStranger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace LoafAndStranger.DataAccess
{
    public class LoafRepository
    {
        const string ConnectionString = "Server=localhost;Database=LoafAndStranger;Trusted_Connection=True;";
        public List<Loaf> GetAll()
        {

            //Create a connection
            using var db = new SqlConnection(ConnectionString);


            //telling the command what you want to do
            var sql = @"Select * 
                        from Loaves";

            return db.Query<Loaf>(sql).ToList(); ;
        }
        public Loaf Get(int id)
        {
            var sql = $@"Select *
                        From Loaves
                        where Id = @id";

            //create a connection
            using var db = new SqlConnection(ConnectionString); //using only works with things that have the IDisposable property, when it's done it calls .dispose

            var loaf = db.QueryFirstOrDefault<Loaf>(sql, new { id = id });

            return loaf;

        }
        public void Add(Loaf loaf)
        {
            using var db = new SqlConnection(ConnectionString);

            var sql = @"INSERT INTO [dbo].[Loaves]([Size],[Type],[WeightInOunces],[Price],[Sliced])
                        OUTPUT inserted.Id
                        VALUES(@Size, @Type, @WeightInOunces, @Price, @Sliced)";

            var id = db.ExecuteScalar<int>(sql, loaf);

            loaf.Id = id;
        }
        public void Remove(int id)
        {
            var sql = @"DELETE 
                       from Loaves
                       where Id = @id";

            using var db = new SqlConnection(ConnectionString);

            db.Execute(sql, new { id });
        }
    }
}
