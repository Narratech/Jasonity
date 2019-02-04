using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import java.util.Iterator;
import java.util.List;

/**
 * The interface for lists of the AgentSpeak language
 *
 * @opt nodefillcolor lightgoldenrodyellow
 *
 */
namespace Jason.Logic.AsSyntax
{
    public interface ListTerm: List<Term>, Term
    {
    }
}
