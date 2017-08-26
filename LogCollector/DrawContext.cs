using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace LogCollector.Application
{
    public class DrawContext : DbContext
    {
        public DrawContext(DbContextOptions<DrawContext> options) : base(options)
        {
        }

        public DbSet<Period> Periods { get; set; }
    }


    public static class DrawContextFactory
    {
        public static DrawContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DrawContext>();
            optionsBuilder.UseMySQL(connectionString);

            var context = new DrawContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }

    [Table("draw_no")]
    public class Period
    {
        [Key]
        [Column("period_no")]
        public long PeriodNo { get; set; }

        [Column("bet_count")]
        public int BetCount { get; set; }
    }
}
