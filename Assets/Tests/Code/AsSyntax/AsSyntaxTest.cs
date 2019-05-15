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
        public void ParseLiteralTestSimplePasses()
        {
            // Use the Assert class to test conditions
            Literal l = new LiteralImpl("likes");
            Literal resultado = AsSyntax.ParseLiteral("likes(john, music).");
            Assert.AreEqual(l, resultado);
        }

        [Test]
        public void ParserNumberTestSimplePasses()
        {
            INumberTerm nti = new NumberTermImpl(12.5);
            INumberTerm resultado = AsSyntax.ParseNumber("12,5");
            Assert.AreEqual(nti.ToString(), resultado.ToString());
        }

        [Test]
        public void ParseVarTermTestSimplePasses()
        {
            VarTerm vt = new VarTerm("A");
            VarTerm resultado = AsSyntax.ParseVar("A");
            Assert.NotNull(resultado);
            Assert.NotNull(vt);
            Assert.IsTrue(vt.IsVar());
            Assert.IsTrue(resultado.IsVar());
            Assert.IsTrue(vt.Equals(vt));
            Assert.IsTrue(vt.Equals(vt));
            Assert.IsTrue(vt.Equals(resultado));
            Assert.AreNotSame(vt, resultado);
            Assert.AreEqual(vt, resultado);
        }

        [Test]
        public void ParseStructureSimplePasses()
        {
            Structure s = new Structure("hello(brother).");
            Structure resultado = AsSyntax.ParseStructure("hello(brother).");
            Assert.AreEqual(s, resultado);
        }

        [Test]
        public void ParseTermSimplePasses()
        {
            ITerm t = new Atom("sister");
            ITerm resultado = AsSyntax.ParseTerm("sister");
            Assert.AreEqual(t, resultado);
        }

        [Test]
        public void ParsePlanSimplePasses()
        {
            Plan p = new Plan();
            Plan resultado = AsSyntax.ParsePlan("+!clear(X):tower([H | T]) & .member(X, T) <- move(H, table); !clear(X).");
            Assert.AreEqual(p, resultado);
        }

        //[Test]
        //public void ParseTriggerSimplePasses()
        //{
        //    Trigger t = new Trigger();
        //}

        [Test]
        public void ParseListTermSimplePasses()
        {
            IListTerm lt = new ListTermImpl();
            IListTerm resultado = AsSyntax.ParseList("[bigfuckinggun]");
            Assert.AreEqual(lt, resultado);
        }

        //[Test]
        //public void ParseRuleSimplePasses()
        //{
        //    Rule r = new Rule();
        //}

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
