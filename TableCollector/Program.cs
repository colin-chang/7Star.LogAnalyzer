using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace TableCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "DrawSqls.sql";
            string file = "/Users/zhangcheng/Desktop/DrawTables.xlsx";

            var sqlTables = GetTables(path);
            var tables = QueryTableInfo(sqlTables);
            GenerateReport(file, tables);

            Console.ReadKey();
        }

        static IEnumerable<string> GetTables(string path)
        {
            string sqls = File.ReadAllText(path).Replace("\r\n", "");
            string regex = @"((INSERT INTO)|UPDATE|(DELETE FROM)|JOIN)\s+(\w+)";
            return ScanSqls(sqls, regex);
        }


        static IEnumerable<string> ScanSqls(string sqls, string regex)
        {
            var tables = new HashSet<string>();
            var matches = Regex.Matches(sqls, regex);
            foreach (Match match in matches)
            {
                tables.Add(match.Groups.LastOrDefault()?.Value);
            }
            return tables;
        }

        static List<Table> QueryTableInfo(IEnumerable<string> tables)
        {
            if (tables == null || !tables.Any())
                return null;

            string connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build().GetConnectionString("TestConnection");
            using (var context = TableContextFactory.Create(connectionString))
            {
                return context.Periods.FromSql($"SELECT TABLE_NAME,TABLE_ROWS,DATA_LENGTH,INDEX_LENGTH FROM TABLES WHERE TABLE_SCHEMA = 'sevenstar_hf_director2' AND TABLE_NAME IN({string.Join(",", tables.Select(t => "'" + t + "'"))}) ORDER BY TABLE_ROWS DESC")?.ToList();
            }
        }

        static void GenerateReport(string file, List<Table> tables)
        {
            if (tables == null || !tables.Any())
                return;

            if (File.Exists(file))
                File.Delete(file);

            int rowNumber = 0;
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(rowNumber);
            row.CreateCell(0, CellType.String).SetCellValue("表名");
            row.CreateCell(1, CellType.String).SetCellValue("数据量");
            row.CreateCell(2, CellType.String).SetCellValue("数据大小");
            row.CreateCell(3, CellType.String).SetCellValue("索引大小");

            tables.ForEach(t =>
            {
                var rw = sheet.CreateRow(++rowNumber);
                rw.CreateCell(0, CellType.String).SetCellValue(t.TableName);
                rw.CreateCell(1, CellType.Numeric).SetCellValue(t.TableRows);
                rw.CreateCell(2, CellType.Numeric).SetCellValue(t.DataLength);
                rw.CreateCell(3, CellType.Numeric).SetCellValue(t.IndexLength);
            });

            using (Stream stream = File.OpenWrite(file))
            {
                workbook.Write(stream);
            }
        }
    }
}
