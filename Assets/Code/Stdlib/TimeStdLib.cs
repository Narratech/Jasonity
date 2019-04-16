using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.ReasoningCycle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
  <p>Internal action: <b><code>.time(HH,MM,SS)</code></b>.
  <p>Description: gets the current time (hour, minute, and seconds).
  <p>Parameters:<ul>
  <li>+/- hours (number): the hours (0--23).</li>
  <li>+/- minutes (number): the minutes (0--59).</li>
  <li>+/- seconds (number): the seconds (0--59).</li>
  </ul>
  <p>Examples:<ul>
  <li> <code>.time(H,M,S)</code>: unifies H with the current hour, M
  with the current minutes, and S with the current seconds.</li>
  <li> <code>.time(15,_,_)</code>: succeeds if it is now 3pm or a bit later
  but not yet 4pm.</li>
  </ul>
 */

namespace Assets.Code.Stdlib
{
    public class TimeStdLib:InternalAction
    {
        private static InternalAction singleton = null;
        public static InternalAction Create()
        {
            if (singleton == null)
                singleton = new TimeStdLib();
            return singleton;
        }

        override public int GetMinArgs()
        {
            return 3;
        }
        override public int GetMaxArgs()
        {
            return 3;
        }

        override public object Execute(Reasoner ts, Unifier un, ITerm[] args)
        {
            CheckArguments(args);
            DateTime myDt = new DateTime();
            Calendar now = new GregorianCalendar();
            return un.Unifies(args[0], new NumberTermImpl(now.GetHour(myDt.Hour))) &&
                un.Unifies(args[1], new NumberTermImpl(now.GetMinute(myDt.Minute))) &&
                un.Unifies(args[2], new NumberTermImpl(now.GetSecond(myDt.Second)));
            //No se como va esto exactamente pero por aquí van los tiros
        }
    }
}
