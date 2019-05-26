using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Code.BDIAgent
{
    [TestFixture]
    public class AgentTest
    {
        [SetUp] // El @Before de Java
        public void SetUp()
        {


        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CreateAgentTest()
        {
            Agent a = Agent.Create(new AgentArchitecture(), "", new Runtime.Settings());
        }
    }
}
