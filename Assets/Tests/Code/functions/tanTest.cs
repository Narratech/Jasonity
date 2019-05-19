using System;
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
    public class tanTest
    {
        tan a;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            a = new tan();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void tanTestGetName()
        {
            // Use the Assert class to test conditions
            string s = "math.tan";
            string resultado = a.GetName();
            Assert.AreEqual(s, resultado);
        }

        [Test]
        public void tanTestEvaluate()
        {
            Reasoner r = new Reasoner(new Agent(), new Circumstance(), new AgentArchitecture(), new Settings());
            ITerm[] args = new ITerm[1];
            args[0] = new NumberTermImpl();

            Assert.AreEqual(Math.Tan(((INumberTerm)args[0]).Solve()), a.Evaluate(r, args));
        }
    }
}
