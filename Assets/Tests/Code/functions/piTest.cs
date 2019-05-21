﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.functions;
using Assets.Code.ReasoningCycle;
using Assets.Code.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Code.functions
{
    public class piTest
    {
        pi a;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            a = new pi();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void piTestGetName()
        {
            // Use the Assert class to test conditions
            string s = "math.pi";
            string resultado = a.GetName();
            Assert.AreEqual(s, resultado);
        }

        [Test]
        public void piTestEvaluate()
        {
            Reasoner r = new Reasoner(new Agent(), new Circumstance(), new AgentArchitecture(), new Settings());
            ITerm[] args = new ITerm[1];
            Assert.AreEqual(Math.PI, a.Evaluate(r, args));            
        }
    }
}