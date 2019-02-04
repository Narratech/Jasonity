using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
import jason.asSemantics.Unifier;
import jason.asSyntax.parser.as2j;

import java.io.Serializable;
import java.io.StringReader;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Base class for all terms.
 *
 * (this class may be renamed to AbstractTerm in future releases of Jason, so
 * avoid using it -- use ASSyntax class to create new terms)
 *
 * @navassoc - source - SourceInfo
 * @opt nodefillcolor lightgoldenrodyellow
 *
 * @see ASSyntax
 */
namespace Jason.Logic.AsSyntax
{
    [Serializable]
    public class DefaultTerm: ArtihFunctionTerm
    {
    }
}
