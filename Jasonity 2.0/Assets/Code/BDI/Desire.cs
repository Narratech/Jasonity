using Assets.Code.Syntax;

namespace Assets.Code.BDI
{
    public class Desire
    {
        private string desire;
        

        public Desire(string d)
        {
            this.desire = d;
        }

        public string GetDesire()
        {
            return desire;
        }

        public void SetDesire(string d)
        {
            desire = d;
        }
    }
}
