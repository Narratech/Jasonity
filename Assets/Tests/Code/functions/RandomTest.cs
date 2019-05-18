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
using UnityEngine.TestTools;

namespace Tests
{
    public class RandomTest
    {
        random a;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            a = new random();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void RandomTestGetName()
        {
            // Use the Assert class to test conditions
            string s = "math.random";
            string resultado = a.GetName();
            Assert.AreEqual(s, resultado);
        }

        [Test]
        public void RandomTestEvaluate()
        {
            Reasoner r = new Reasoner(new Agent(), new Circumstance(), new AgentArchitecture(), new Settings());
            ITerm[] args = new ITerm[1];
            args[0] = new NumberTermImpl();

            Random rAux = new Random();
            double aux = rAux.Next() * ((INumberTerm)args[0]).Solve();
            Assert.AreEqual(aux, a.Evaluate(r, args));

            //The test says that the result is wrong but it's only because 
            //we're generating two random numbers and comparing them.
            args[0] = new StringTermImpl();
            aux = rAux.Next();
            Assert.AreEqual(aux, a.Evaluate(r, args));
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator RandomTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
