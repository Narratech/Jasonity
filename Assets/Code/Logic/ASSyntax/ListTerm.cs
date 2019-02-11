using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * The interface for lists of the AgentSpeak language
 *
 */

namespace Jason.Logic.AsSyntax
{
    public interface ListTerm: Term, IList<Term>
    {
        void SetTerm(Term t);
        Term GetTerm();
        void SetNext(Term l);

        bool IsEnd();

        bool IsTail();
        VarTerm GetTail();
        void SetTail(VarTerm v);
        ListTerm GetLast();
        ListTerm GetPenultimate();
        Term RemoveLast();
        ListTerm Append(Term t);
        ListTerm Ansert(Term t);

        ListTerm Concat(ListTerm lt);
        ListTerm Reverse();

        ListTerm Union(ListTerm lt);
        ListTerm Intersection(ListTerm lt);
        ListTerm Difference(ListTerm lt);

        /** returns all subsets that take k elements of this list */
        IEnumerator<List<Term>> SubSets(int k);


        IEnumerator<ListTerm> ListTermIterator();
        List<Term> GetAsList();

        /** clone the list term */
        ListTerm CloneLT();

        /** make a shallow copy of the list (terms are not cloned, only the structure) */
        ListTerm CloneLTShallow();
    }
}
