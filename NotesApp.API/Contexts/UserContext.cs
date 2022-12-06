using Microsoft.EntityFrameworkCore;
using NotesApp.API.Entities;
using System;

namespace NotesApp.API.Contexts
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    UserId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    FirstName = "Nikola",
                    LastName = "Kostic",
                    Email = "nikola@gmail.com",
                    Password = "123"
                });

            modelBuilder.Entity<Note>().HasData(
                new Note()
                {
                    NoteId = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                    Title = "Prva beleska",
                    Content = "Ovo je moja prva beleska",
                    Color = "blue",
                    UserId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35")
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
