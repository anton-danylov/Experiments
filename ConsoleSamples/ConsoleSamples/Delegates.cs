using System;
using System.Linq;

namespace ConsoleSamples
{
    public class Delegates
    {
        delegate bool SomeDelegate(string name);

        SomeDelegate _t1;

        public void Do()
        {
            _t1 += (s) => { Console.WriteLine($"1: {s}"); return true; };
            _t1 += (s) => { Console.WriteLine($"2: {s}"); return true; };
            _t1 += (s) => { Console.WriteLine($"3: {s}"); throw null; };
            _t1 += (s) => { Console.WriteLine($"4: {s}"); return true; };


            try
            {
                _t1("invoke");
            }
            catch { }

            //1: invoke
            //2: invoke
            //3: invoke

            _t1.GetInvocationList().ToList().ForEach(d =>
            {
                try
                {
                    (d as SomeDelegate).Invoke("from list");
                }
                catch { }
            });

            //1: from list
            //2: from list
            //3: from list
            //4: from list
        }

        public void Actions()
        {
            Action<string> action = (s) => Console.WriteLine($"From action: {s}");
            action += (s) => Console.WriteLine($"From action 2: {s}");
            action += (s) => Console.WriteLine($"From action 3: {s}");

            action += delegate { Console.WriteLine("Anonimous"); };

            action("preved");

            //From action: preved
            //From action 2: preved
            //From action 3: preved
        }
    }
}
