using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * The interface for lists of the AgentSpeak language
 *
 * @opt nodefillcolor lightgoldenrodyellow
 *
 */
namespace Assets.Code.Logic.AsSyntax
{
    public interface ListTerm:Term
    {
        void SetTerm(Term t);
        Term GetTerm();
        void SetNext(Term l);
        ListTerm GetNext();

        bool IsEnd();

        bool IsTail();
        VarTerm GetTail();
        void SetTail(VarTerm v);
        ListTerm GetLast();
        ListTerm GetPenultimate();
        Term RemoveLast();
        ListTerm Append(Term t);
        ListTerm Insert(Term t);

        ListTerm Concat(ListTerm lt);
        ListTerm Reverse();

        ListTerm Union(ListTerm lt);
        ListTerm Intersection(ListTerm lt);
        ListTerm Difference(ListTerm lt);

        //Return all subsets that take k elements of this list
        IEnumerator<List<Term>> SubSets(int k);

        IEnumerator<ListTerm> ListTermIterator();
        List<Term> GetAsList();

        //Clone the list term
        ListTerm CloneLT();

        //Make a shallow copy of the list (terms are not cloned, only the structure)
        ListTerm CloneLTShallow();
        void Add(Pred pred);
        void Add(LogicalFormula logicalFormula);
        Term Get(int v);
        void Add(Trigger trigger);
        void Add(PlanBody planBody);
    }
}
