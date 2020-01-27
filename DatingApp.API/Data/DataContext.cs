using Microsoft.EntityFrameworkCore;
using DatingApp.API.models;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        //EF CORE會幫我們把DbSet轉成資料表
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
    }
}