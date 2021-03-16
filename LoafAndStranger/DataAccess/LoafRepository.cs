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
        static List<Loaf> _loaves = new List<Loaf>
            {
                 new Loaf { Id = 1, Price = 5.50m, Size = LoafSize.Medium, Sliced = true, Type= "Rye"},
                 new Loaf { Id = 2, Price = 2.50m, Size = LoafSize.Small, Sliced = false, Type= "French"},
            };

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
        public void Add(Loaf loaf)
        {
            var biggestExistingId = _loaves.Max(l => l.Id);
            loaf.Id = biggestExistingId + 1;
            _loaves.Add(loaf);
        }
        public Loaf Get(int id)
        {
            var sql = $@"Select *
                        From Loaves
                        where Id = @Id";

            //create a connection
            using var connection = new SqlConnection(ConnectionString);
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

        public void Remove(int id)
        {
            var loafToRemove = Get(id);
            _loaves.Remove(loafToRemove);
        }
    }
}
