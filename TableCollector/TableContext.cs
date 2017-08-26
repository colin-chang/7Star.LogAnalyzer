using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace TableCollector
{
    public class TableContext : DbContext
    {
        public TableContext(DbContextOptions<TableContext> options) : base(options)
        {
        }

        public DbSet<Table> Periods { get; set; }
    }


    public static class TableContextFactory
    {
        public static TableContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TableContext>();
            optionsBuilder.UseMySQL(connectionString);

            var context = new TableContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }

    [Table("TABLES")]
    public class Table
    {
        [Key]
        [Column("TABLE_NAME")]
        public string TableName { get; set; }

        [Column("TABLE_ROWS")]
        public long TableRows { get; set; }

        [Column("DATA_LENGTH")]
        public long DataLength { get; set; }

        [Column("INDEX_LENGTH")]
        public long IndexLength { get; set; }
    }
}
