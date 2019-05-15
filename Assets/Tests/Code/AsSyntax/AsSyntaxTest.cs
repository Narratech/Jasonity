using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [TestFixture]
    public class AsSyntaxTest
    {
        AsSyntax asS;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            asS = new AsSyntax();
            
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }

        // A Test behaves as an ordinary method
        [Test]
        public void ParseLiteralTestSimplePasses()
        {
            // Use the Assert class to test conditions
            Literal resultado = AsSyntax.ParseLiteral("likes(john, music).");
            
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ParseLiteralTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
