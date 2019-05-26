using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * The interface for numeric terms of AgentSpeak language
 */
namespace Assets.Code.AsSyntax
{
    public interface INumberTerm:ITerm
    {
        /** returns the numeric value of the term */
        double Solve();
    }
}
