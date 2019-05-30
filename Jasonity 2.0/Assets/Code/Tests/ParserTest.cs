using System.Collections;
using System.Collections.Generic;
using Assets.Code.Syntax;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Code.Test
{
    
    [TestFixture]
    public class ParserTest
    {
        private Parser prueba;

        [SetUp] // El @Before de Java
        public void SetUp()
        {
            prueba = new Parser();
        }

        [TearDown] // El @After de Java (ahora mismo no lo estoy usando)
        public void TearDown()
        {
            
        }

        [Test]
        public void ParseTest()
        {

            //Test goal
            prueba.Parse("!light_off(lamp)");

            //Test test goal
            prueba.Parse("?light_off(lamp)");

            /*Test plan add
             - If plan contains a goal
             - If plan contains a test goal
             - If plan contains a belief
             */
            prueba.Parse("+light_off(lamp):true<-.moverALampara(),.!TurnOnLight(),.volverACama().");

            prueba.Parse("+light_off(lamp):true<-.moverALampara(),.?TurnOnLight(),.volverACama().");

            // ESTA
            prueba.Parse("+light_off(lamp):true<-.moverALampara(),.TurnOnLight(),.volverACama().");

            /*Test plan delete
             - If plan contains a goal
             - If plan contains a test goal
             - If plan contains a belief
             */
            prueba.Parse("-light_off(lamp):true<-.moverALampara(),.!TurnOnLight(),.volverACama().");

            prueba.Parse("-light_off(lamp):true<-.moverALampara(),.?TurnOnLight(),.volverACama().");

            prueba.Parse("-light_off(lamp):true<-.moverALampara(),.TurnOnLight(),.volverACama().");

            //Test default
            prueba.Parse("light_off(lamp).");

            //Test no point at the end
            prueba.Parse(".moverALampara(),");

            //Preparar prueba para opcion ESTA
        }

        [Test]
        public void ParsePlanTest()
        {
            char @operator = '+';
            string trigger = "trigger";
            string[] parameters = new string[2];
            parameters[0] = "p1";
            parameters[1] = "p2";

            List<Action> actions = new List<Action>();
            string[] args1 = new string[2];
            string[] args2 = new string[2];
            args1[0] = "arg1";
            args1[1] = "arg2";
            args2[0] = "arg1";
            args2[1] = "arg2";

            Action a1 = new Action("a1", args1, ',');
            Action a2 = new Action("a2", args2, ';');
            actions.Add(a1);
            actions.Add(a2);

            Trigger t = new Trigger(@operator, trigger, parameters);
            string context = "context";
            PlanBody pBody = new PlanBody(actions);
            
            Plan p = new Plan(t, context, pBody);


        }

    }
}
