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
            un.function.Add(new VarTerm("key"), new VarTerm("value"));
            string s = "key";

            ITerm resultado = un.Get(s);

            VarTerm v = new VarTerm("value");

            Assert.AreEqual(v, resultado);
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
            un.function.Add(new VarTerm("key"), new VarTerm("value"));

            VarTerm k = new VarTerm("key");

            VarTerm v = new VarTerm("value");

            ITerm resultado = un.Get(k);

            Assert.AreEqual(v, resultado);
        }

        [Test]
        public void GetVarFromValueTest()
        {
            //Tengo probar con el diccionario function pero no me deja tocarlo
            un.function.Add(new VarTerm("key"), new VarTerm("value"));

            ITerm v = new VarTerm("value");

            ITerm k = new VarTerm("key");

            VarTerm resultado = un.GetVarFromValue(v);

            Assert.AreEqual(k, resultado);
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
            //HACER pruebas mas severas
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
            un.function.Add(new VarTerm("key"), new VarTerm("value"));

            VarTerm v = new VarTerm("key");

            VarTerm funcion = un.Deref(v);

            bool resultado = un.function.ContainsKey(v);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void BindTest()
        {
            VarTerm v = new VarTerm("key");
            ITerm t = new LiteralImpl(false, "value");
            
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
            un.function.Add(new VarTerm("key"), new VarTerm("value"));

            un.Clear();

            bool resultado = un.function.ContainsKey(new VarTerm("key"));

            Assert.AreEqual(false, resultado);
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
            un.function.Add(new VarTerm("key1"), new VarTerm("value1"));
            un.function.Add(new VarTerm("key2"), new VarTerm("value2"));
            un.function.Add(new VarTerm("key3"), new VarTerm("value3"));

            int resultado = un.Size();

            Assert.AreEqual(3, resultado);
        }

        [Test]
        public void CloneTest()
        {
            Unifier resultado = un.Clone();

            Assert.AreEqual(un, resultado);
        }

        [Test]
        public void EqualsTest()
        {
            Unifier unifier = new Unifier();

            un.function.Add(new VarTerm("key1"), new VarTerm("value1"));
            unifier.function.Add(new VarTerm("key1"), new VarTerm("value1"));

            bool resultado = un.Equals(unifier);

            Assert.AreEqual(true, resultado);
        }

        [Test]
        public void SetDictionaryTest()
        {
            Dictionary<VarTerm, ITerm> diccionario = new Dictionary<VarTerm, ITerm>();

            diccionario.Add(new VarTerm("key1"), new VarTerm("value1"));

            un.SetDictionary(diccionario);

            Assert.AreEqual(un, diccionario);
        }

        [Test]
        public void GetEnumeratorTest()
        {
            Assert.AreEqual();
        }
    }
}
