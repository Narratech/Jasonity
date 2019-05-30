using Assets.Code.Syntax;

namespace Assets.Code.BDI
{
    public class Belief
    {
        private Term belief;
        private string percept;
        private string value;

        public string GetBelief()
        {
            return percept + ":" + value;
        }

        public Belief(Term b)
        {
            this.belief = b;
        }

        public Belief(string b)
        {

        }
    }
}