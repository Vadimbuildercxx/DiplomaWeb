using Diploma.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace Diploma.Models
{
    public class DBContext : IdentityDbContext
    {
        private DbSet<Person> _persons;
        private DbSet<Log> _logs;
        private DbSet<Camera> _cameras;
        private DbSet<Area> _areas;

        protected readonly IConfiguration Configuration;

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get => _persons; set => _persons = value; }
        public DbSet<Log> Logs { get => _logs; set => _logs = value; }
        public DbSet<Camera> Cameras { get => _cameras; set => _cameras = value; }
        public DbSet<Area> Areas { get => _areas; set => _areas = value; }
    }
}
