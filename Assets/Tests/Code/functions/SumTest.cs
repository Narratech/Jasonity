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
    public class SumTest
    {
        Sum a;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            a = new Sum();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void SumTestGetName()
        {
            // Use the Assert class to test conditions
            string s = "math.sum";
            string resultado = a.GetName();
            Assert.AreEqual(s, resultado);
        }

        [Test]
        public void SumTestEvaluate()
        {
            Reasoner r = new Reasoner(new Agent(), new Circumstance(), new AgentArchitecture(), new Settings());
            ITerm[] args = new ITerm[1];
            args[0] = new NumberTermImpl();

            double auxSum = 0;
            foreach (ITerm t in (IListTerm)args[0])
            {
                if (t.IsNumeric())
                {
                    auxSum += ((INumberTerm)t).Solve();
                }
            }
            Assert.AreEqual(auxSum, a.Evaluate(r, args));
            
        }
    }
}
