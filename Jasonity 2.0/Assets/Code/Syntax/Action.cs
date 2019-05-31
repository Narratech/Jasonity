namespace Assets.Code.Syntax
{
    public class Action
    {
        private string actionName;
        private string[] arguments;
        //Symbol can be a ",", ";","&" or a "|".
        //Is the last character of the line.
        private char symbol;

        public Action(string a, string[] args, char symbol)
        {
            actionName = a;
            arguments = args;
            this.symbol = symbol;
        }

        public string ActionName { get => actionName; }
        public string[] Arguments { get => arguments; }
        public char Symbol { get => symbol; }
    }
}
