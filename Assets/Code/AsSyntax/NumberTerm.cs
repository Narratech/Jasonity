using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * The interface for numeric terms of AgentSpeak language
 */
namespace Assets.Code.Logic.AsSyntax
{
    public interface NumberTerm:Term
    {
        /** returns the numeric value of the term */
        double Solve();
    }
}
