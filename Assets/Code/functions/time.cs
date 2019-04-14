using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using System;
using Assets.Code.AsSemantics;

namespace Assets.Code.functions
{
    public partial class time : ArithFunction
    {

        public override string GetName()
        {
            return "math.time";
        }

        public override double Evaluate(Reasoner reasoner, ITerm[] args)
        {
            return TimeUtils.CurrentTimeMillis();
        }

        public override bool CheckArity(int a)
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