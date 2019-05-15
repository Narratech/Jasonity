using System.Collections;
using System.Collections.Generic;
using Assets.Code.AsSyntax;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Code.functions
{
    
    [TestFixture]
    public class AbsTest
    {
        Abs abs;
        ITerm[] vector;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            abs = new Abs();
            vector = new ITerm[1];
            vector[0] = new NumberTermImpl(-3);
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
        }
        

        /* Los métodos de test (dentro se utiliza la clase Assert para probar que se cumplen las condiciones) */
        [Test]
        public void TestAbs1()
        {
            double resultado = abs.Evaluate(null, vector);
            Assert.AreEqual(8.0d, resultado);
        }


        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestAtomWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }




    }
}
