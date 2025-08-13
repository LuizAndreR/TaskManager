using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data.Mappings;

internal class TaskMap : IEntityTypeConfiguration<TaskE>
{
    public void Configure(EntityTypeBuilder<TaskE> builder)
    {
        builder.ToTable("Tarefas");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Descriptions)
            .HasMaxLength(400);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasMaxLength(5)
            .HasDefaultValue("Media");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(15)
            .HasDefaultValue("Pendente");

        builder.Property(t => t.DateCreated)
            .IsRequired()
            .HasColumnType("date");

        builder.HasOne(t => t.Usuario)
            .WithMany(u => u.Tarefas)
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
