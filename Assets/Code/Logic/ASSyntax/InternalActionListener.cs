﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 A particular type of literal used to represent internal actions (which has a "." in the functor).

 @navassoc - ia - InternalAction

 */
namespace Jason.Logic.AsSyntax
{
    public class InternalActionListener : Structure, LogicalFormula
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Term other)
        {
            throw new NotImplementedException();
        }
    }
}
