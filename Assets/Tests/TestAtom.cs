using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Code.functions
{
    [TestFixture]
    public class TestAtom
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestAtomSimplePasses()
        {
            // Use the Assert class to test conditions

        }

        /* CREO QUE NO VAMOS A USARLOS
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestAtomWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        */
    }
}
