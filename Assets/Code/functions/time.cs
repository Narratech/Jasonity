using Assets.Code.AsSyntax;
using System;

namespace Assets.Code.functions
{
    public partial class time : DefaultArithFunction
    {

        public string GetName()
        {
            return "math.time";
        }

        public double Evaluate(TransitionSystem ts, ITerm[] args)
        {
            return TimeUtils.CurrentTimeMillis();
        }

        public bool CheckArity(int a)
        {
            return a == 0;
        }

        public class TimeUtils
        {
            private static readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            public static long CurrentTimeMillis()
            {
                return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
            }
        }
    }
}