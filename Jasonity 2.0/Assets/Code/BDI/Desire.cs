using Assets.Code.Syntax;

namespace Assets.Code.BDI
{
    public class Desire
    {
        private Term desire;
        string typeOfTest;

        public Desire(Term d, string s)
        {
            this.desire = d;
            this.typeOfTest = s;
        }
    }
}
