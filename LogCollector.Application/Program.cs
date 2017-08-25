using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace LogCollector.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            //注册编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string dir = "/Users/zhangcheng/Desktop/Logs";
            string path = "/Users/zhangcheng/Desktop/log.xlsx";

            ScanLogs(dir, path);

            Console.WriteLine("Done");
        }

        /// <summary>
        /// 扫描日志并生成报表
        /// </summary>
        /// <param name="dir">Dir.</param>
        static void ScanLogs(string dir, string path)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            long totalElapse = 0;

            var logs = Directory.GetFiles(dir);
            var records = new List<DrawRecord>();
            Console.WriteLine($"扫描 {dir} 目录日志文件完成，耗时{watch.ElapsedMilliseconds}...");
            totalElapse += watch.ElapsedMilliseconds;
            watch.Restart();


            foreach (string log in logs)
                records.AddRange(CollectRecords(log));
            Console.WriteLine($"日志采集完成，耗时{watch.ElapsedMilliseconds}...");
            totalElapse += watch.ElapsedMilliseconds;
            watch.Restart();


            GetBetCounts(records.Select(r => r.PeriodNo))
                .ForEach(p => records.FirstOrDefault(r => r.PeriodNo == p.PeriodNo).BetCount = p.BetCount);
            Console.WriteLine($"查询数据库注单量完成，耗时{watch.ElapsedMilliseconds}...");
            totalElapse += watch.ElapsedMilliseconds;
            watch.Restart();


            GenerateReport(records, path);
            Console.WriteLine($"报表生成完成，耗时{watch.ElapsedMilliseconds}...");
            totalElapse += watch.ElapsedMilliseconds;
            watch.Stop();


            Console.WriteLine($"全部任务完成，总耗时{totalElapse}...");
        }

        /// <summary>
        /// 采集日志开奖记录
        /// </summary>
        /// <returns>The records.</returns>
        /// <param name="path">Path.</param>
        static List<DrawRecord> CollectRecords(string path)
        {
            //读取文件
            var logs = File.ReadAllLines(path, Encoding.GetEncoding("gb2312")).Where(l => !string.IsNullOrWhiteSpace(l));
            var results = new List<DrawRecord>();

            //定义正则
            string regBegin = "^.+自动开奖启动.+?DBTime=(.+)\\),总监2,(\\d{11})期.+$",
            regAsync = @"调用异步：(\d+)",
            regLock = @"查询锁表：(\d+)",
            regCollect = @"收集语句：(\d+)",
            regExec = @"批量执行：(\d+).+语句数：(\d+)",
            regTran = @"事物提交：(\d+)",
            regMs = @"主从延时：(\d+)",
            regTotal = @"开奖累计：(\d+)";

            for (int i = 0; i < logs.Count(); i++)
            {
                string line = logs.ElementAt(i);
                var match = Regex.Match(line, regBegin);
                if (!match.Success)
                {
                    continue;
                }

                //获取数据行
                string asyncLn = logs.ElementAt(++i),
                lockLn = logs.ElementAt(++i),
                collectLn = logs.ElementAt(++i),
                execLn = logs.ElementAt(++i),
                tranLn = logs.ElementAt(++i),
                msLn = logs.ElementAt(++i),
                totalLn = logs.ElementAt(++i);

                //数据提取
                Match matchAsync = Regex.Match(asyncLn, regAsync),
                matchLock = Regex.Match(lockLn, regLock),
                matchCollect = Regex.Match(collectLn, regCollect),
                matchExec = Regex.Match(execLn, regExec),
                matchTran = Regex.Match(tranLn, regTran),
                matchMs = Regex.Match(msLn, regMs),
                matchTotal = Regex.Match(totalLn, regTotal);

                results.Add(new DrawRecord(
                    Convert.ToInt64(match.Groups[2].Value),
                    Convert.ToInt32(matchAsync.Groups[1].Value),
                    Convert.ToInt32(matchLock.Groups[1].Value),
                    Convert.ToInt32(matchCollect.Groups[1].Value),
                    Convert.ToInt32(matchExec.Groups[2].Value),
                    Convert.ToInt32(matchExec.Groups[1].Value),
                    Convert.ToInt32(matchTran.Groups[1].Value),
                    Convert.ToInt32(matchMs.Groups[1].Value),
                    Convert.ToInt32(matchTotal.Groups[1].Value),
                    Convert.ToDateTime(match.Groups[1].Value)
                ));
            }

            return results;
        }

        /// <summary>
        /// 获取注单量
        /// </summary>
        /// <returns>The bet counts.</returns>
        /// <param name="periods">Periods.</param>
        static List<Period> GetBetCounts(IEnumerable<long> periods)
        {
            if (periods == null || periods.Count() <= 0)
                return null;

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("TestConnection");

            using (var context = DrawContextFactory.Create(connectionString))
            {
                return context.Periods.FromSql($"SELECT period_no,bet_count FROM draw_no where period_no in ({string.Join(",", periods)})")?.ToList();
            }
        }

        /// <summary>
        /// 生成报表
        /// </summary>
        /// <param name="records">Records.</param>
        static void GenerateReport(List<DrawRecord> records, string file)
        {
            IWorkbook workbook;
            ISheet sheet;
            int startRowNumber;
            long existMax = 0;

            try
            {
                if (File.Exists(file))
                {
                    using (Stream stream = File.OpenRead(file))
                    {
                        workbook = new XSSFWorkbook(stream);
                    }
                    sheet = workbook.GetSheetAt(0);
                    startRowNumber = sheet.LastRowNum;
                    existMax = (long)sheet.GetRow(startRowNumber).GetCell(0).NumericCellValue;
                    File.Delete(file);
                }
                else
                {
                    workbook = new XSSFWorkbook();
                    sheet = workbook.CreateSheet();
                    var row = sheet.CreateRow(0);
                    row.CreateCell(0, CellType.String).SetCellValue("期号");
                    row.CreateCell(1, CellType.String).SetCellValue("异步耗时");
                    row.CreateCell(2, CellType.String).SetCellValue("锁表耗时");
                    row.CreateCell(3, CellType.String).SetCellValue("收集SQL耗时");
                    row.CreateCell(4, CellType.String).SetCellValue("SQL语句数量");
                    row.CreateCell(5, CellType.String).SetCellValue("注单量");
                    row.CreateCell(6, CellType.String).SetCellValue("批量执行SQL耗时");
                    row.CreateCell(7, CellType.String).SetCellValue("事务提交耗时");
                    row.CreateCell(8, CellType.String).SetCellValue("主从耗时");
                    row.CreateCell(9, CellType.String).SetCellValue("开奖累计耗时");
                    row.CreateCell(10, CellType.String).SetCellValue("开奖时间");

                    startRowNumber = 0;
                }

                var style = workbook.CreateCellStyle();
                style.DataFormat = HSSFDataFormat.GetBuiltinFormat("m/d/yy h:mm");
                records.ForEach(r =>
                {
                    if (r.PeriodNo > existMax)
                    {
                        var row = sheet.CreateRow(++startRowNumber);
                        row.CreateCell(0, CellType.Numeric).SetCellValue(r.PeriodNo);
                        row.CreateCell(1, CellType.Numeric).SetCellValue(r.AsyncElapse);
                        row.CreateCell(2, CellType.Numeric).SetCellValue(r.LockElapse);
                        row.CreateCell(3, CellType.Numeric).SetCellValue(r.CollectElapse);
                        row.CreateCell(4, CellType.Numeric).SetCellValue(r.SqlsCount);
                        row.CreateCell(5, CellType.Numeric).SetCellValue(r.BetCount);
                        row.CreateCell(6, CellType.Numeric).SetCellValue(r.ExecElapse);
                        row.CreateCell(7, CellType.Numeric).SetCellValue(r.TransElapse);
                        row.CreateCell(8, CellType.Numeric).SetCellValue(r.MsElapse);
                        row.CreateCell(9, CellType.Numeric).SetCellValue(r.TotalElapse);

                        var cell = row.CreateCell(10);
                        cell.CellStyle = style;
                        cell.SetCellValue(r.DrawDateTime);
                    }
                });

                using (Stream stream = File.OpenWrite(file))
                {
                    workbook.Write(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
