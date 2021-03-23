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

            //////Loop through data while associating it -- slower if you have lots of data, because it makes potentially unnecessary calls while looping

            //var strangersSql = "Select * from strangers where topid = @id";

            //foreach (var top in tops)
            //{
            //    var relatedStrangers = db.Query<Stranger>(strangersSql, top);
            //    top.Strangers = relatedStrangers.ToList();
            //}

            ///////Get all data then loop through it to associate strangers and tops -- faster but a bit harder to read
            var topsSql = @"Select * from Tops";

            var strangerSql = "select * from strangers where topid is not null";

            var tops = db.Query<Top>(topsSql);
            var strangers = db.Query<Stranger>(strangerSql);

            foreach (var top in tops)
            {
                top.Strangers = strangers.Where(s => s.TopId == top.Id).ToList();
            }

            ///////Group By Method -- interesting but even harder to read

            //var groupedStrangers = strangers.GroupBy(s => s.TopId);

            //foreach (var groupedStranger in groupedStrangers)
            //{
            //    tops.First(t => t.Id == groupedStranger.Key).Strangers = groupedStranger.ToList();
            //}

            ///////Lookup Method -- very similar to group by but more obscure

            //var groupedStrangers = strangers.ToLookup(s => s.TopId);

            //foreach (var groupedStranger in groupedStrangers)
            //{
            //    tops.First(t => t.Id == groupedStranger.Key).Strangers = groupedStranger.ToList();
            //}

            return tops;
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
