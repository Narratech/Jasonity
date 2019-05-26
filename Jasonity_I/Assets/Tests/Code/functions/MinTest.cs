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
    public class MinTest
    {
        Min a;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            a = new Min();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void MinTestGetName()
        {
            // Use the Assert class to test conditions
            string s = "math.min";
            string resultado = a.GetName();
            Assert.AreEqual(s, resultado);
        }

        [Test]
        public void MinTestEvaluate()
        {
            Reasoner r = new Reasoner(new Agent(), new Circumstance(), new AgentArchitecture(), new Settings());
            ITerm[] args = new ITerm[2];
            args[0] = new NumberTermImpl();
            args[1] = new NumberTermImpl();

            double n0 = ((INumberTerm)args[0]).Solve();
            double n1 = ((INumberTerm)args[1]).Solve();
            double minAux = Math.Min(n0, n1);
            Assert.AreEqual(minAux, a.Evaluate(r, args));
            //hasta aquí funciona

            args[0] = new ListTermImpl();
            minAux = double.MaxValue;
            foreach (ITerm item in (IListTerm)args[0])
            {
                if (item.IsNumeric())
                {
                    double n = ((INumberTerm)item).Solve();
                    if (n > minAux)
                        minAux = n;
                }
            }
            Assert.AreEqual(minAux, a.Evaluate(r, args));
        }
    }
}
