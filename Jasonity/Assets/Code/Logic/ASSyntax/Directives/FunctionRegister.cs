using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Agent;
import jason.asSemantics.ArithFunction;
import jason.asSyntax.NumberTerm;
import jason.asSyntax.Pred;
import jason.asSyntax.StringTerm;
import jason.functions.Abs;
import jason.functions.Average;
import jason.functions.Length;
import jason.functions.Max;
import jason.functions.Min;
import jason.functions.Random;
import jason.functions.Round;
import jason.functions.Sqrt;
import jason.functions.StdDev;
import jason.functions.Sum;
import jason.functions.ceil;
import jason.functions.e;
import jason.functions.floor;
import jason.functions.log;
import jason.functions.pi;
import jason.functions.time;

import java.util.HashMap;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * This class maintains the set of arithmetic functions available for the AS parser.
 *
 */

namespace Jason.Logic.AsSyntax.Directives
{
    public class FunctionRegister: DefaultDirective, Directive
    {

    }
}
