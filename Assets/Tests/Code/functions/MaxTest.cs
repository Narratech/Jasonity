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

namespace Tests
{
    public class MaxTest
    {
        Max a;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            a = new Max();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void MaxTestGetName()
        {
            // Use the Assert class to test conditions
            string s = "math.max";
            string resultado = a.GetName();
            Assert.AreEqual(s, resultado);
        }

        [Test]
        public void MaxTestEvaluate()
        {
            Reasoner r = new Reasoner(new Agent(), new Circumstance(), new AgentArchitecture(), new Settings());
            ITerm[] args = new ITerm[2];
            args[0] = new NumberTermImpl();
            args[1] = new NumberTermImpl();

            double n0 = ((INumberTerm)args[0]).Solve();
            double n1 = ((INumberTerm)args[1]).Solve();
            double maxAux = Math.Max(n0, n1);
            Assert.AreEqual(maxAux, a.Evaluate(r, args));
            //hasta aquí funciona

            args[0] = new ListTermImpl();
            maxAux = double.MinValue;
            foreach (ITerm item in (IListTerm)args[0])
            {
                if (item.IsNumeric())
                {
                    double n = ((INumberTerm)item).Solve();
                    if (n > maxAux)
                        maxAux = n;
                }
            }
            Assert.AreEqual(maxAux, a.Evaluate(r, args));
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator MaxTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
