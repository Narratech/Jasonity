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
     * Common interface for all kind of terms.
     */
    public interface ITerm: IEquatable<ITerm>, IComparable<ITerm>, IComparable {
        // Not only IComparable<ITerm> interface is needed, but also IEquatable<ITerm> and IComparable. 
        // Indeed more interfaces should be implemented: something similar to Java Cloneable, Serializable and ToDOM

        /* Returns true only if this term is a variable (or a variable term?). */
        bool IsVar();

        /* Returns true only if this term is an unnamed variable. */
        bool IsUnnamedVar();

        /* Returns true only if this term is a literal. */
        bool IsLiteral();

        /* Returns true only if this term is a rule. */
        bool IsRule();

        /* Returns true only if this term is a list. */
        bool IsList();

        /* Returns true only if this term is a string. */
        bool IsString();

        /* Returns true only if this term is an internal action. */
        bool IsInternalAction();

        /* Returns true only if this term is an arithmetic expression. */
        bool IsArithExpr();

        /* Returns true only if this term is a numeric term. */
        bool IsNumeric();

        /* Returns true only if this term is a predicacte. */
        bool IsPred();

        /* Returns true only if this term is a ground term. */
        bool IsGround();

        /* Returns true only if this term is a structure. */
        bool IsStructure();

        /* Returns true only if this term is an atom. */
        bool IsAtom();

        /* Returns true only if this term is a plan body. */
        bool IsPlanBody();

        /* Returns true only if this term is a cyclic term. */
        bool IsCyclicTerm();

        /*************************/

        /* 
         * Returns true only if this term contains the variable t (or unifies with the variable term t?). 
         * t - The variable term
         * u - The unifier
         */
        bool HasVar(VarTerm t, Unifier u);

        /* 
         * Returns the cyclic variable term (if this is a cyclic variable term?)
         */ 
        VarTerm GetCyclicVar();

        /* 
         * Modifies the dictionary c adding the number of variables contained in this term?
         * c - The dictionary of variable terms and their number of references
         */
        void CountVars(Dictionary<VarTerm, int?> c);

        /*
         * Clone this term. We have to implement a ICloneable or even our own IDeepCloneable interface!
         */
        //ITerm Clone();

        /*
         * Returns true if other object o is equal to this term. We have already implemented IEquatable interface.
         * o - The other object
         */
        //bool Equals(object o);

        /*
         * Returns true if this term subsumes other term l.
         * l - The other term
        */
        bool Subsumes(ITerm l);

        /** 
         * There is no apply method, which replaces variables by their values in the unifier, returning true if some variable was applied 
         */
        //public bool Apply(Unifier u);
        
        /** 
         * Returns a clone of this term with changes applied (it replaces variables by their value in the unifier), both operations together.
         * This double operation should be faster than 'Clone' and then 'Apply'. 
         * u - The unifier
         */
        ITerm CApply(Unifier u);

        /** 
         * Returns a clone of this term in another namespace. 
         * ns - The new namespace
         */
        ITerm CloneNS(Atom ns);


        // These two methods should be transformed in a single property
        //object SrcInfo { get; set;  }

        /** 
         * Sets the source info of this term. 
         * s - The source info
         */  
        void SetSrcInfo(SourceInfo s);

        /** 
          * Gets the source info of this term. 
          */
        SourceInfo GetSrcInfo();
    }
}
