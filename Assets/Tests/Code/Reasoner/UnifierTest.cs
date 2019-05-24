using Assets.Code.AsSyntax;
using Assets.Code.ReasoningCycle;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class UnifierTest
    {


        [SetUp] // El @Before de Java
        public void SetUp()
        {


        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {

        }

        [Test]
        public void GetStringTest()
        {
            //Tengo probar con el diccionario function pero no me deja tocarlo
            Unifier.function.Add(new VarTerm("key"), new VarTerm("value"));

            string v = "key";

            ITerm resultado = Unifier.Get(v);

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
            IEnumerator<VarTerm> e = Unifier.Enumerator();

            Assert.AreEqual();
        }

        [Test]
        public void GetVarTermTest()
        {
            //Tengo probar con el diccionario function pero no me deja tocarlo
            Unifier.function.Add(new VarTerm("key"), new VarTerm("value"));

            string v = "value";

            Assert.AreEqual();
        }

        [Test]
        public void GetVarFromValueTest()
        {
            //Tengo probar con el diccionario function pero no me deja tocarlo
            Unifier.function.Add(new VarTerm("key"), new VarTerm("value"));

            ITerm t = new VarTerm("value");

            VarTerm resultado = Unifier.GetVarFromValue(t);

            Assert.AreEqual("key", resultado);
        }

        [Test]
        public void UnifiesTriggerTest()
        {
            Trigger t = new Trigger(TEOperator.add, TEType.belief, new LiteralImpl("literal"));

            bool resultado = Unifier.Unifies(t, t);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void UnifiesNoUndoTriggerTest()
        {
            Trigger t = new Trigger(TEOperator.add, TEType.belief, new LiteralImpl("literal"));

            bool resultado = Unifier.UnifiesNoUndo(t, t);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void UnifiesITermTest()
        {
            ITerm t = null;

            bool resultado = Unifier.Unifies(t, t);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void CloneFunctionTest()
        {
            //Comprobar que el clon es igual que el diccionario functions al que no me deja acceder
            Dictionary<VarTerm, ITerm> diccionario = Unifier.CloneFunction();

            Assert.AreEqual(diccionario, Unifier.function);
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
            bool resultado = Unifier.UnifiesNamespace(l, l);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void DerefTest()
        {
            //Necesito el diccionario function de Unifier que no me deja tocarlo
            VarTerm v = new VarTerm("variable");

            VarTerm resultado = Unifier.Deref(v);

            Assert.AreEqual();
        }

        [Test]
        public void BindTest()
        {
            VarTerm v = new VarTerm("variable");
            ITerm t = new VarTerm("termino");

            bool resultado = Unifier.Bind(v, t);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void GetVarForUnifierTest()
        {
            //Funcion mal hecha
            VarTerm v = Unifier.GetVarForUnifier(new VarTerm("term"));

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
            ITerm resultado = Unifier.GetAsTerm();

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
