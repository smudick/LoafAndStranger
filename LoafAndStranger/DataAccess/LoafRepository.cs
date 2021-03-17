using LoafAndStranger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LoafAndStranger.DataAccess
{
    public class LoafRepository
    {
        const string ConnectionString = "Server=localhost;Database=LoafAndStranger;Trusted_Connection=True;";
        public List<Loaf> GetAll()
        {

            var loaves = new List<Loaf>();

            //Create a connection
            using var connection = new SqlConnection(ConnectionString);

            //open the connection
            connection.Open();

            //create a command
            var command = connection.CreateCommand();

            //telling the command what you want to do
            command.CommandText = @"Select * 
                                    from Loaves";

            //send the command to sql
            //execute the command
            var reader = command.ExecuteReader();

            //loop over results
            while (reader.Read()) //reader.read pulls one row at a time from the database
            {

                //add it to the list
                loaves.Add(MapLoaf(reader));
            }

            return loaves;
        }
        public Loaf Get(int id)
        {
            var sql = $@"Select *
                        From Loaves
                        where Id = @Id";

            //create a connection
            using var connection = new SqlConnection(ConnectionString); //using only works with things that have the IDisposable property, when it's done it calls .dispose
            //Open it
            connection.Open();
            //create a command
            var command = connection.CreateCommand();
            command.CommandText = sql;
            //this protects you from sql injection -- value in quotes must match value in command text
            command.Parameters.AddWithValue("Id", id);
            //execute command
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var loaf = MapLoaf(reader);
                return loaf;
            }
            return null;
        }
        private Loaf MapLoaf(SqlDataReader reader)
        {
            var id = (int)reader["Id"]; //explicit cast (on fail throws exception)
            var size = (LoafSize)reader["Size"];
            var type = reader["Type"] as string; //implicit cast (on fail returns null)
            var price = (decimal)reader["Price"];
            var sliced = (bool)reader["sliced"];
            var createdDate = (DateTime)reader["createdDate"];
            var weightInOunces = (int)reader["weightInOunces"];

            //make a loaf
            var loaf = new Loaf()
            {
                Id = id,
                Price = price,
                Size = size,
                Type = type,
                Sliced = sliced,
                WeightInOunces = weightInOunces
            };
            return loaf;
        }
        public void Add(Loaf loaf)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO [dbo].[Loaves]([Size],[Type],[WeightInOunces],[Price],[Sliced])
                                OUTPUT inserted.Id
                                VALUES(@Size, @Type, @WeightInOunces, @Price, @Sliced)";

            cmd.Parameters.AddWithValue("Size", loaf.Size);
            cmd.Parameters.AddWithValue("Type", loaf.Type);
            cmd.Parameters.AddWithValue("WeightInOunces", loaf.WeightInOunces);
            cmd.Parameters.AddWithValue("Price", loaf.Price);
            cmd.Parameters.AddWithValue("Sliced", loaf.Sliced);

            var id = (int)cmd.ExecuteScalar();

            loaf.Id = id;
        }
        public void Remove(int id)
        {

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE 
                                from Loaves
                                WHERE Id = @Id";

            cmd.Parameters.AddWithValue("Id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
