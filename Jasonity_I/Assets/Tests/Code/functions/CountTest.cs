﻿using System.Collections;
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
    public class CountTest
    {
        Count a;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            a = new Count();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CountTestGetName()
        {
            // Use the Assert class to test conditions
            string s = ".count";
            string resultado = a.GetName();
            Assert.AreEqual(s, resultado);
        }

        [Test]
        public void CountTestEvaluate()
        {
            Reasoner r = new Reasoner(new Agent(), new Circumstance(), new AgentArchitecture(), new Settings());
            ITerm[] args = new ITerm[1];
            args[0] = new NumberTermImpl();

            double result = a.Evaluate(r, args);
            Assert.AreEqual(0, result);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator CountTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
