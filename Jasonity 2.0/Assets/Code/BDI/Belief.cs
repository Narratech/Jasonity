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

        //public Belief(Term b)
        //{
        //    this.belief = b;
        //}
        public Belief(string s)
        {

        }

        public Belief(string p, string v)
        {
            percept = p;
            value = v;
        }
        
        public void UpdatePercept(string s)
        {
            percept = s;
        }

        public void UpdateValue(string s)
        {
            value = s;
        }

        public string GetPercepts()
        {
            return percept;
        }

        public string GetValue()
        {
            return value;
        }
    }
}