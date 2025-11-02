using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WormReads.Data;
using WormReads.Models;

namespace WormReads.DataAccess.Repository.User_Rpository
{
    public class UserRepository : Repository<User> , IUserRpository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
