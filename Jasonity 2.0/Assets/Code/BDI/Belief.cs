using Assets.Code.Syntax;

namespace Assets.Code.BDI
{
    public class Belief
    {
        private string name;
        private string value;

        public string GetBelief()
        {
            return name + ":" + value;
        }

        public Belief(string p, string v)
        {
            name = p;
            value = v;
        }
        
        public void UpdateName(string s)
        {
            name = s;
        }

        public void UpdateValue(string s)
        {
            value = s;
        }

        public string GetName()
        {
            return name;
        }

        public string GetValue()
        {
            return value;
        }
    }
}