using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Entities;

namespace TmsApi.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(150);


            builder.Property<DateTime>("LastUpdated");

builder.Property(s => s.Version)
       .IsRowVersion();


    // Exercise 9 - hide deleted students
    builder.HasQueryFilter(s => !s.IsDeleted);
    
    }
}