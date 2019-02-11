using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Represents a list node as in prolog .(t1,.(t2,.(t3,.))).
 *
 * Each nth-ListTerm has both a term and the next ListTerm.
 * The last ListTem is an empty ListTerm (term==null).
 * In lists terms with a tail ([a|X]), next is the Tail (next==X, term==a).
 *
 * @navassoc - element - Term
 * @navassoc - next - ListTerm
 *
 */
namespace Jason.Logic.AsSyntax
{
    public class ListTermImpl : Structure, ListTerm
    {
        public Term this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(Term item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Term other)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Term item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Term[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Term> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(Term item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Term item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Term item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
