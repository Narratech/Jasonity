/*    
    Jasonity (2018-2019)
    http://www.narratech.com/jasonity
    This file is part of the Jasonity project, a reimplementation of Jason, the Java platform for BDI agents, in Unity / C#.
    Original project: http://jason.sf.net
    License: GNU Lesser General Public License v3.0

    Authors: Jaime Bas, Álvaro Cuevas, Alejandro García, Juan Gómez-Martinho and Irene González
    Supervisor: Federico Peinado 
    Contact: info@narratech.com
*/
namespace Assets.Code.AsSyntax { // Change to Narratech.Jasonity.AsSyntax

    using System;
    using System.Collections.Generic;
    using Assets.Code.ReasoningCycle;

    /**
     * Base class for all terms.
     *
     * (this class may be renamed to AbstractTerm in the future, so avoid using it -- use ASSyntax class to create new terms)
     */
    [Serializable]
    public abstract class DefaultTerm: ITerm {

        // Cached hash code for not recalculating it
        public int hashCodeCache = -1;

        // The source info of this term
        public SourceInfo srcInfo = null;

        /*
         * Returns a clone of this term. We have to implement a ICloneable or even our own IDeepCloneable interface!
         */
        abstract public object Clone();
        // Probably another Clone method is needed where ITerm is returned, not object

        /*
         * Returns the calculated hash code of this term. Probably there is no need for this method existing GetHashCode!
         */
        abstract public int CalcHashCode();

        /**************************************************************************/

        /*
         * Returns a clone of this term with changes applied (it replaces variables by their value in the unifier), both operations together.
         * This double operation should be faster than 'Clone' and then 'Apply'. 
         * u - The unifier
         */
        public virtual ITerm CApply(Unifier u) {
            // It call Clone specific method, not (ITerm)MemberwiseClone()
            return (ITerm)Clone(); // Como uso el Clone de C# lo que clono son object que luego hay que castear...
        }

        /*
         * Returns a clone of this term in another namespace. 
         * ns - The new namespace
         */
        public virtual ITerm CloneNS(Atom newNamespace) {
            // It call Clone specific method, not (ITerm)MemberwiseClone()
            return (ITerm)Clone(); // Como uso el Clone de C# lo que clono son object que luego hay que castear...
        }

        /*
         * Returns true if this term is equal to other term t.
         * This method should be overriden by specific terms.
         */
        public virtual bool Equals(ITerm t) {
            if (t == null)
                return false;
            return this.ToString().Equals(t.ToString());
        }

        /*
         * Returns true if this term is equal to other object o.
         */
        public override bool Equals(object o) {
            if (o is ITerm)
                return Equals(o as ITerm);
            return false;
        }

        /*
        // Redefinición de los operadores de igualdad 
        public static bool operator ==(Node left, Node right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }
        public static bool operator !=(Node left, Node right)
        {
            if (ReferenceEquals(left, null))
                return !ReferenceEquals(right, null);
            return !left.Equals(right);
        }
        */

        /*
         * Compares this term with other term t, for sorting.
         */
        public virtual int CompareTo(ITerm t) {
            if (t == null) {
                return -1;
            } else {
                return this.ToString().CompareTo(t.ToString());
            }
        }

        /*
         * Compares this term with other object o, for sorting.
         * This is implemented in C# for retro-compatibility with the old IComparable interface.
         */
        int IComparable.CompareTo(object o) {
            if (!(o is ITerm))
                throw new ArgumentException("Argument is not a Term", "o");
            ITerm t = (ITerm)o;
            return CompareTo(t);
        }

        /*
        // Redefinición de los operadores relacionales en base a la comparación en coste, para ordenar
        public static bool operator <(Node left, Node right) => left.CompareTo(right) < 0;
        public static bool operator >(Node left, Node right) => left.CompareTo(right) > 0;
        public static bool operator <=(Node left, Node right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Node left, Node right) => left.CompareTo(right) >= 0;
        */

        /*
         * Returns the hash code for this term.  
         * This is useful for optimizing access in collections, it should be a fast calculation.
         */
        public override int GetHashCode()
        {
            if (hashCodeCache == -1)
                hashCodeCache = CalcHashCode(); // Not calling base.GetHashCode() but calculating the hash code if it has not been calculated yet
            return hashCodeCache;
        }

        /*
         * Returns the representative string for this term.
         */
        public override string ToString() {
            return "default"; // By default, this is the representation of a default term (it should be overriden)
        }

        /***************************************************************************/

        /* 
         * Modifies the dictionary c adding the number of variables contained in this term?
         * c - The dictionary of variable terms and their number of references
         */
        public virtual void CountVars(Dictionary<VarTerm, int?> c) {
            // No changes are performed
        }

        /* 
         * Returns the cyclic variable term (if this is a cyclic variable term?)
         */
        public virtual VarTerm GetCyclicVar() {
            return null;
        }

        /* 
         * Gets the source info of this term. 
         */
        public virtual SourceInfo GetSrcInfo() {
            return srcInfo;
        }

        /* 
         * Sets the source info of this term. 
         * s - The source info
         */
        public virtual void SetSrcInfo(SourceInfo s) {
            srcInfo = s;
        }

        /*
         * Returns the error message of this term
         */
        public virtual string GetErrorMsg() {
            if (srcInfo == null) {
                return "";
            } else {
                return srcInfo.ToString();
            }
        }

        /*
         * Resets the hash code for this term to its 'no calculated' value.   
         */
        public void ResetHashCodeCache() {
            hashCodeCache = -1;
        }

        /*
         * Returns true if this term subsumes other term l.
         * l - The other term
         */
        public virtual bool Subsumes(ITerm l) {
            if (l.IsVar()) {
                return false;
            } else {
                return true;
            }
        }

        /****** Checks *******/

        /* 
         * Returns true only if this term contains the variable t (or unifies with the variable term t?). 
         * t - The variable term
         * u - The unifier
         */
        public virtual bool HasVar(VarTerm t, Unifier u) {
            return false;
        }

        /* Returns true only if this term is an arithmetic expression. */
        public virtual  bool IsArithExpr()
        {
            return false;
        }

        /* Returns true only if this term is an atom. */
        public virtual bool IsAtom()
        {
            return false;
        }

        /* Returns true only if this term is a cyclic term. */
        public virtual bool IsCyclicTerm()
        {
            return false;
        }

        /* Returns true only if this term is a ground term. */
        public virtual bool IsGround()
        {
            return true;
        }

        /* Returns true only if this term is an internal action. */
        public virtual bool IsInternalAction()
        {
            return false;
        }

        /* Returns true only if this term is a list. */
        public virtual bool IsList()
        {
            return false;
        }

        /* Returns true only if this term is a literal. */
        public virtual bool IsLiteral()
        {
            return false;
        }

        /* Returns true only if this term is a numeric term. */
        public virtual bool IsNumeric()
        {
            return false;
        }

        /* Returns true only if this term is a plan body. */
        public virtual bool IsPlanBody()
        {
            return false;
        }

        /* Returns true only if this term is a predicacte. */
        public virtual bool IsPred()
        {
            return false;
        }

        /* Returns true only if this term is a rule. */
        public virtual bool IsRule()
        {
            return false;
        }

        /* Returns true only if this term is a string. */
        public virtual bool IsString()
        {
            return false;
        }

        /* Returns true only if this term is a structure. */
        public virtual bool IsStructure()
        {
            return false;
        }

        /* Returns true only if this term is an unnamed variable. */
        public virtual bool IsUnnamedVar()
        {
            return false;
        }

        /* Returns true only if this term is a variable (or a variable term?). */
        public virtual bool IsVar()
        {
            return false;
        }     
    }
}
