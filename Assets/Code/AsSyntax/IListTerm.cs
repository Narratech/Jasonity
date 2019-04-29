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
namespace Assets.Code.AsSyntax
{
    public interface IListTerm: IList<ITerm>, ITerm 
    {
        void SetTerm(ITerm t);
        ITerm GetTerm();
        void SetNext(ITerm l);
        IListTerm GetNext();

        bool IsEnd();

        bool IsTail();
        VarTerm GetTail();
        void SetTail(VarTerm v);
        IListTerm GetLast();
        IListTerm GetPenultimate();
        ITerm RemoveLast();
        IListTerm Append(ITerm t);
        IListTerm Insert(ITerm t);

        IListTerm Concat(IListTerm lt);
        IListTerm Reverse();

        IListTerm Union(IListTerm lt);
        IListTerm Intersection(IListTerm lt);
        IListTerm Difference(IListTerm lt);

        int Size();

        //Return all subsets that take k elements of this list
        IEnumerator<List<ITerm>> SubSets(int k);

        IEnumerator<IListTerm> ListTermIterator();
        List<ITerm> GetAsList();

        //Clone the list term
        IListTerm CloneLT();

        //Make a shallow copy of the list (terms are not cloned, only the structure)
        IListTerm CloneLTShallow();
    }
}
