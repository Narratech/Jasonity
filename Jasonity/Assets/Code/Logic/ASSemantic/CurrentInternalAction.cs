using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**

  This class can be used in place of DefaultInternalAction to create an IA that
  suspend the intention while it is being executed.

  Example: a plan may ask something to an user and wait the answer.
  If DefaultInternalAction is used for that, all the agent thread is blocked until
  the answer. With ConcurrentInternalAction, only the intention using the IA is
  suspended. See demos/gui/gui1.

  The code of an internal action that extends this class looks like:

  <pre>
  public class ...... extends ConcurrentInternalAction {

    public Object execute(final TransitionSystem ts, Unifier un, final Term[] args) throws Exception {
        ....

        final String key = suspendInt(ts, "gui", 5000); // suspend the intention (max 5 seconds)

        startInternalAction(ts, new Runnable() { // to not block the agent thread, start a thread that performs the task and resume the intention latter
            public void run() {

                .... the code of the IA .....

                if ( ... all Ok ...)
                    resumeInt(ts, key); // resume the intention with success
                else
                    failInt(ts, key); // resume the intention with fail
            }
        });

        ...
    }

    public void timeout(TransitionSystem ts, String intentionKey) { // called back when the intention should be resumed/failed by timeout (after 5 seconds in this example)
        ... this method have to decide what to do with actions finished by timeout: resume or fail
        ... to call resumeInt(ts,intentionKey) or failInt(ts, intentionKey)
    }
  }
  </pre>
*/
namespace Logica.ASSemantic
{
    public abstract class CurrentInternalAction: InternalAction
    {
    }
}
