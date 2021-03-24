using Dapper;
using LoafAndStranger.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoafAndStranger.DataAccess
{
    public class TopsRepository
    {
        AppDbContext _db;
        public TopsRepository(AppDbContext db)
        {
            _db = db;
        }
        public IEnumerable<Top> GetAll()
        {

            return _db.Tops.Include(t => t.Strangers)
                .ThenInclude(s => s.Loaf).AsNoTracking();
        }    
        public IEnumerable<Top> GetAllOccupied()
        {
            return _db.Tops.Where(t => t.Occupied);
        }

        public Top Add(int numberOfSeats)
        {
            var top = new Top { NumberOfSeats = numberOfSeats };

            _db.Tops.Add(top);

            _db.SaveChanges();

            return top;
        }
    }
}
