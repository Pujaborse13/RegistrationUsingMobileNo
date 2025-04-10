using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Context
{
    public class FundooDBContext : DbContext
    {

        public FundooDBContext(DbContextOptions option) : base(option) { }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<NotesEntity> Notes { get; set; }
        public DbSet<CollaboratorEntity> Collaborator { get; set; }

        public DbSet<LabelEntity> Labels{ get; set; }

        public DbSet<NoteLabelEntity> NoteLabels{ get; set; }

        public DbSet<UserRegistrationEntity> UserRegistration { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many relationship between NoteEntity and LabelEntity
            modelBuilder.Entity<NoteLabelEntity>()
                .HasKey(nl => new { nl.NoteId, nl.LabelId });

            modelBuilder.Entity<NoteLabelEntity>()
                .HasOne(nl => nl.Note)
                .WithMany(n => n.NoteLabel)
                .HasForeignKey(nl => nl.NoteId)
                .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction



            modelBuilder.Entity<NoteLabelEntity>()
                .HasOne(nl => nl.Label)
                .WithMany(l => l.NoteLabel)
                .HasForeignKey(nl => nl.LabelId)
                 .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction


        }


    }
}

