using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
      // public StudentSystemContext(DbContextOptions contextOptions)
      // {
      //
      // }


        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-FKR965V\SQLEXPRESS;Database=StudentSystem;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigStudent(modelBuilder);

            ConfigCourse(modelBuilder);

            ConfigResource(modelBuilder);

            ConfigHomework(modelBuilder);

            ConfigStudentCourse(modelBuilder);
        }

        private void ConfigStudentCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>().HasKey(ck => new { ck.StudentId, ck.CourseId });

            modelBuilder.Entity<StudentCourse>().HasOne(s => s.Student).WithMany(c => c.CourseEnrollments).HasForeignKey(fk => fk.StudentId);

            modelBuilder.Entity<StudentCourse>().HasOne(c => c.Course).WithMany(s => s.StudentsEnrolled).HasForeignKey(fk => fk.CourseId);
        }

        private void ConfigHomework(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(h => h.HomeworkId);

                entity.HasOne(s => s.Student).WithMany(h => h.HomeworkSubmissions).HasForeignKey(s => s.StudentId);

                entity.HasOne(c => c.Course).WithMany(hs => hs.HomeworkSubmissions).HasForeignKey(c => c.CourseId);
            });
        }

        private void ConfigResource(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.ResourceId);

                entity.Property(n => n.Name).HasMaxLength(50).IsUnicode();

                entity.HasOne(c => c.Course).WithMany(r => r.Resources).HasForeignKey(c => c.CourseId);
            });
        }

        private void ConfigCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.CourseId);

                entity.Property(n => n.Name).HasMaxLength(80).IsUnicode();

                entity.Property(d => d.Description).IsUnicode();
            });
        }

        private void ConfigStudent(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Student>()
                .HasKey(s => s.StudentId);

            modelBuilder
                .Entity<Student>()
                .Property(n => n.Name)
                .HasMaxLength(100)
                .IsUnicode()
                .IsRequired();

            modelBuilder
                .Entity<Student>()
                .Property(pn => pn.PhoneNumber)
                .HasColumnType("CHAR(10)");

            modelBuilder
                .Entity<Student>()
                .Property(r => r.RegisteredOn)
                .HasDefaultValueSql("getdate()");
        }
    }
}
