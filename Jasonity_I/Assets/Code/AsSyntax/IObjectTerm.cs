﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.AsSyntax
{
    public interface IObjectTerm:ITerm
    {
        object GetObject();
    }
}