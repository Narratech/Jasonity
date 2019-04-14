using Assets.Code.AsSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Assets.Code.ReasoningCycle;
using Assets.Code.Agent;

/*
 * Description: gets the current date (year, month, and day of the month).
 */
namespace Assets.Code.Stdlib
{
    public class DateStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new DateStdLib();
            return singleton;
        }

        public override int GetMinArgs()
        {
            return 3;
        }

        public override int GetMaxArgs()
        {
            return 3;
        }

        public override object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            DateTime dt = new DateTime();
            GregorianCalendar now = new GregorianCalendar();
            return un.Unifies(args[0], new NumberTermImpl(now.GetYear(dt))) &&
                   un.Unifies(args[0], new NumberTermImpl(now.GetMonth(dt) + 1)) &&
                   un.Unifies(args[0], new NumberTermImpl(now.GetDayOfMonth(dt)));
        }
    }
}
