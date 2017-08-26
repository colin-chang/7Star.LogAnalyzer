using System;
namespace LogCollector
{
    public class DrawRecord
    {
        public long PeriodNo
        {
            get;
            set;
        }

        public int AsyncElapse
        {
            get;
            set;
        }

        public int LockElapse
        {
            get;
            set;
        }

        public int CollectElapse
        {
            get;
            set;
        }

        public int SqlsCount
        {
            get;
            set;
        }

        public int BetCount
        {
            get;
            set;
        }

        public int ExecElapse
        {
            get;
            set;
        }

        public int TransElapse
        {
            get;
            set;
        }

        public int MsElapse
        {
            get;
            set;
        }

        public int TotalElapse
        {
            get;
            set;
        }

        public DateTime DrawDateTime
        {
            get;
            set;
        }

        public DrawRecord(long periodNo, int asyncElapse, int lockElapse, int collectElapse, int sqlsCount, int execElapse, int transElapse, int msElapse, int totalElapse, DateTime drawDateTime)
        {
            this.PeriodNo = periodNo;
            this.AsyncElapse = asyncElapse;
            this.LockElapse = lockElapse;
            this.CollectElapse = collectElapse;
            this.SqlsCount = sqlsCount;
            this.ExecElapse = execElapse;
            this.TransElapse = transElapse;
            this.MsElapse = msElapse;
            this.TotalElapse = totalElapse;
            this.DrawDateTime = drawDateTime;
        }
    }
}
