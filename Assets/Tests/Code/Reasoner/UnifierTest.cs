using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class UnifierTest
    {
        private Unifier un; 

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            un = new Unifier();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {

        }

        [Test]
        public void GetStringTest()
        {
            //Tengo probar con el diccionario function pero no me deja tocarlo
            un.function.Add(new VarTerm("key"), new VarTerm("value"));
            string v = "key";

            ITerm resultado = un.Get(v);

            Assert.AreEqual("value", resultado);
        }

        [Test]
        public void RemoveTest()
        {
            Dictionary<VarTerm, ITerm> diccionario = new Dictionary<VarTerm, ITerm>();

            diccionario.Add(new VarTerm("key"), new VarTerm("value"));

            bool resultado = diccionario.Remove(new VarTerm("key"));

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void EnumeratorTest()
        {
            //No se que debería devolver el GetEnumerator de function.Key
            IEnumerator<VarTerm> e = un.Enumerator();

            Assert.AreEqual();
        }

        [Test]
        public void GetVarTermTest()
        {
            //Tengo probar con el diccionario function pero no me deja tocarlo
            un.function.Add(new VarTerm("key"), new VarTerm("value"));

            string v = "value";

            Assert.AreEqual();
        }

        [Test]
        public void GetVarFromValueTest()
        {
            //Tengo probar con el diccionario function pero no me deja tocarlo
            un.function.Add(new VarTerm("key"), new VarTerm("value"));

            ITerm t = new VarTerm("value");

            VarTerm resultado = un.GetVarFromValue(t);

            Assert.AreEqual("key", resultado);
        }

        [Test]
        public void UnifiesTriggerTest()
        {
            Trigger t = new Trigger(TEOperator.add, TEType.belief, new LiteralImpl("literal"));

            bool resultado = un.Unifies(t, t);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void UnifiesNoUndoTriggerTest()
        {
            Trigger t = new Trigger(TEOperator.add, TEType.belief, new LiteralImpl("literal"));

            bool resultado = un.UnifiesNoUndo(t, t);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void UnifiesITermTest()
        {
            ITerm t = null;

            bool resultado = un.Unifies(t, t);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void CloneFunctionTest()
        {
            //Comprobar que el clon es igual que el diccionario functions al que no me deja acceder
            Dictionary<VarTerm, ITerm> diccionario = un.CloneFunction();

            Assert.AreEqual(diccionario, un.function);
        }


        //No se que hace este metodo
        [Test]
        public void UnifiesNoUndoITermTest()
        {
            Assert.AreEqual();
        }

        //No se que hace este metodo
        [Test]
        public void UnifyTermsTest()
        {
            Assert.AreEqual();
        }

        [Test]
        public void UnifiesNamespaceTest()
        {
            Literal l = new LiteralImpl("literal");
            bool resultado = un.UnifiesNamespace(l, l);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void DerefTest()
        {
            //Necesito el diccionario function de Unifier que no me deja tocarlo
            VarTerm v = new VarTerm("variable");

            VarTerm resultado = un.Deref(v);

            Assert.AreEqual();
        }

        [Test]
        public void BindTest()
        {
            VarTerm v = new VarTerm("variable");
            ITerm t = new VarTerm("termino");

            bool resultado = un.Bind(v, t);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void GetVarForUnifierTest()
        {
            //Funcion mal hecha
            VarTerm v = un.GetVarForUnifier(new VarTerm("term"));

            Assert.AreEqual();
        }

        [Test]
        public void ClearTest()
        {
            //Clear del diccionario function que no me deja tocar

            Assert.AreEqual();
        }

        [Test]
        public void ToStringTest()
        {
            //ToString del diccionario function que no me deja tocar

            Assert.AreEqual();
        }

        [Test]
        public void GetAsTermTest()
        {
            ITerm resultado = un.GetAsTerm();

            //Tiene que hacer esto?
            Assert.AreEqual(new ListTermImpl(), resultado);
        }

        [Test]
        public void SizeTest()
        {
            //Count a diccionario function que no me deja tocar
            Assert.AreEqual();
        }

        [Test]
        public void ComposeTest()
        {
            Assert.AreEqual();
        }

        [Test]
        public void CloneTest()
        {
            Assert.AreEqual();
        }

        [Test]
        public void HashCodeTest()
        {
            Assert.AreEqual();
        }

        [Test]
        public void EqualsTest()
        {
            Assert.AreEqual();
        }

        [Test]
        public void SetDictionaryTest()
        {
            Assert.AreEqual();
        }

        [Test]
        public void GetFunctionTest()
        {
            Assert.AreEqual();
        }

        [Test]
        public void GetEnumeratorTest()
        {
            Assert.AreEqual();
        }
    }
}
