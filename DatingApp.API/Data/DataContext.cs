using Microsoft.EntityFrameworkCore;
using DatingApp.API.models;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        // calling parent class's constructor method.
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        //EF CORE會幫我們把DbSet轉成資料表
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // we have to make the primary key be these to keys.
            //which one user is allowed to like another users once.
            
            builder.Entity<Like>()
                .HasKey(k => new { k.LikerId, k.LikeeId});
            
            builder.Entity<Like>()
                // Like table
                .HasOne(u => u.Likee)
                // user table
                .WithMany( u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

                            
            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany( u => u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                // confiure the property in the like
                .HasOne(m => m.Recipient)
                .WithMany(u => u.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}