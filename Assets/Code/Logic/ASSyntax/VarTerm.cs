using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Represents a variable Term: like X (starts with upper case). It may have a
 * value, after {@link VarTerm}.apply.
 *
 * An object of this class can be used in place of a
 * Literal, Number, List, String, .... It behaves like a
 * Literal, Number, .... just in case its value is a Literal,
 * Number, ...
 *
 */
namespace Jason.Logic.AsSyntax
{
    public class VarTerm : LiteralImpl, NumberTerm, ListTerm
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
