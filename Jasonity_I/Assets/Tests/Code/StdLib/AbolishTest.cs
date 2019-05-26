using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using Assets.Code.BDIAgent;
using Assets.Code.BDIManager;
using Assets.Code.ReasoningCycle;
using Assets.Code.Runtime;
using Assets.Code.Stdlib;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [TestFixture]
    public class AbolishTest
    {
        ITerm[] vector;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            vector = new ITerm[3];
            vector[0] = new NumberTermImpl(-3);
            vector[1] = new VarTerm("Daniel");
            vector[2] = new Pred("hello(brother)[nice(brother)]");
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }


        // A Test behaves as an ordinary method
        [Test]
        public void AbolishTestSimplePasses()
        {
            // Use the Assert class to test conditions
            AbolishStdLib asl = new AbolishStdLib();
            asl.Execute(new Reasoner(new Agent(), new Circumstance(), new AgentArchitecture(), new Settings()), new Unifier(), vector);
            // Uff, aquí es que se crean muchas cosas... es mejor empezar probando a meter nulls donde se pueda

        }
    }
}
