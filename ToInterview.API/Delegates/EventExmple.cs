namespace ToInterview.API.Delegates
{

    public class CatShoutEventArgs : EventArgs
    {
        public string Name { get; set; }
    }

    public class Cat()
    {
        public delegate void CatShoutHandler(object sender, CatShoutEventArgs args);

        public event CatShoutHandler CatShout;

        public void Shout()
        {
            Console.WriteLine("Cat shout");

            var e = new CatShoutEventArgs { Name = "Kitty" };
            CatShout(this, e);
        }
    }

    public class Mouse
    {
        public void Run(object sebder, CatShoutEventArgs args)
        {
            Console.WriteLine($"{args.Name}, Mouse run");
        }
    }
}
