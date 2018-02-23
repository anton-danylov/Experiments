using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSamples
{
    interface IAdd
    {
        void Add(int i);
    }

    struct AddStruct : IAdd
    {
        private int _field;

        void IAdd.Add(int i)
        {
            _field += i;
        }

        public void Add(int i)
        {
            _field += i;
        }

        public void Print(string msg)
        {
            Console.WriteLine($"{msg} : {_field}");
        }

        public static void Add<T>(ref T s, int i)
        {
            (s as IAdd).Add(i);
        }

        public static void AddConstrained<T>(ref T s, int i) where T : IAdd
        {
            s.Add(i);
        }
    }

    class Program
    {

        static void Main(string[] args)
        {


            CountDiscIntersections();

            //ChangeString();
            //Interning();
            //DelegateRecursion();
            //ReflectionTest();
            //PrimeNumbersRanges();
            //TaskCompletionSourceTest();
            //PrimeNumbersTaskTest();
            //TaskExceptions();
            //MultithreadingSamples();
            //TaskCombinators();
            //TypeConverters();
            //WordCount();
            //TestValueTypeInterfaceBoxing();
            //TestCultureInfo();
            //TestEnums();
            //ClosuresTest();
            //DelegatesTest();
            //AnonType();
            //TristateBoolExample();
            //OverrideBool();
            //Dump();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static void CountDiscIntersections()
        {
            DiscIntersectionCounter counter = new DiscIntersectionCounter();

            int[] A1 = { 1, 5, 2, 1, 4, 0 };

            int[] A2 = { 1, 2147483647, 0 };

            int[] A3 = { 1, 0, 1, 0, 1 };

            int[] A4 = { 7, 2, 1, 15, 11, 5, 0, 3, 2, 1 };


            var data = new int[][]{ A1, A2, A3, A4 };

            foreach (var array in data)
            {
                int count = counter.Solution(array);
                int countN2 = counter.SolutionN2(array);

                Console.WriteLine($"{count} == {countN2} : {count == countN2}");
            }
        }

        public unsafe static void ChangeString()
        {
            //var fieldInfo = typeof(string).GetField("m_firstChar", BindingFlags.NonPublic | BindingFlags.Instance);
            //var firstChar = (char)fieldInfo.GetValue(immutable);

            string immutable = "vot tebe i immutable";
            var hash1 = immutable.GetHashCode();
            unsafe
            {
                fixed (char* p = immutable)
                {
                    p[0] = 'p';
                    p[1] = 'u';
                    p[2] = 'j';
                }
            }

            var hash2 = immutable.GetHashCode();
            Console.WriteLine($"{immutable} | {hash1} ? {hash2} ");


            string immutable2 = "vot tebe i immutable";
            var handle = GCHandle.Alloc(immutable2, GCHandleType.Pinned);
            var addr = handle.AddrOfPinnedObject();

            "zvezda".Select((c, i) => new { c = c, i = i }).ToList().ForEach(p => Marshal.WriteInt16(addr, p.i * sizeof(char), p.c));
            handle.Free();

            Console.WriteLine(immutable2);
        }


        public static void DelegateRecursion()
        {
            Func<int, int> fact = null;
            fact = x => (x == 0) ? 1 : x * fact(x - 1);

            // Make a new reference to the factorial function
            Func<int, int> myFact = fact;

            // Use the new reference to calculate the factorial of 4
            var r1 = myFact(4); // returns 24

            // Modify the old reference
            fact = x => x;

            // Again, use the new reference to calculate
            var r2 = myFact(4); // returns 12

            Console.WriteLine($"{r1} : {r2}");
        }


        public static void Interning()
        {
            String s1 = "qwerty";
            String s2 = new String("qwerty".ToArray());
            String s3 = new String(new[] { 'q' });

            String.Intern(s3);

            Console.WriteLine($"{Object.ReferenceEquals(s1, s2)}, {String.IsInterned(s3)} ");
        }

        //===================================================================================
        public class PrivateMethodClass
        {
            private string _field = null;
            public PrivateMethodClass(int i) => _field = "int" + i.ToString();
            public PrivateMethodClass(string s) => _field = "string" + s;
            public PrivateMethodClass(object o) => _field = "object" + Convert.ToString(o);


            public void PrintFld() => Console.WriteLine(_field);
            private void PrintMsg(string msg) => Console.WriteLine($"!{_field}!Message: {msg}");
        }

        public static void ReflectionTest()
        {
            // create delegate from private method with reflection

            PrivateMethodClass pmc = new PrivateMethodClass((object)null);

            var mi = pmc.GetType().GetTypeInfo().GetMethod("PrintMsg", BindingFlags.Instance | BindingFlags.NonPublic);

            mi.Invoke(pmc, new[] { "From MethodInfo" });

            Action<string> act = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), pmc, mi);
            act("From delegate");


            var typeString = typeof(string);
            var typeObject = typeof(object);

            Console.WriteLine($"typeString.IsAssignableFrom(typeObject) => {typeString.IsAssignableFrom(typeObject)}"); //False
            Console.WriteLine($"typeObject.IsAssignableFrom(typeString) => {typeObject.IsAssignableFrom(typeString)}"); //True

            Console.WriteLine($"typeString.IsInstanceOfType(typeObject) => {typeString.IsInstanceOfType(typeObject)}"); //False
            Console.WriteLine($"typeObject.IsInstanceOfType(typeString) => {typeObject.IsInstanceOfType(typeString)}"); //True


            var pmc0 = Activator.CreateInstance(typeof(PrivateMethodClass), "111");
            var pmc1 = Activator.CreateInstance(typeof(PrivateMethodClass), 111);
            //var pmc2 = Activator.CreateInstance(typeof(PrivateMethodClass), null); // throws System.MissingMethodException

            var pmcCi = typeof(PrivateMethodClass).GetConstructor(new[] { typeof(string) });
            var pmc3 = (PrivateMethodClass)pmcCi.Invoke(new[] { (object)null });
            pmc3.PrintFld();

        }

        // ===========================  PrimeNumbersRanges() ============================================
        private static int GetPrimesCount(int start, int count)
        {
            return ParallelEnumerable.Range(start, count).Count(n =>
                Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i != 0));
        }

        private static Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() => ParallelEnumerable.Range(start, count).Count(n =>
                Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i != 0)));
        }

        private class PrimeNumbersStateMachine
        {
            TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

            private void PrintPrimeNumbersRangeAt(int current, int total)
            {
                const int step = 100000;

                var awaiter = GetPrimesCountAsync(current * step + 2, step).GetAwaiter();

                awaiter.OnCompleted(() =>
                {
                    Console.WriteLine($"[{current * step,8}; {(current + 1) * step - 1,8}] => {awaiter.GetResult()}");

                    if (current < total - 1)
                    {
                        PrintPrimeNumbersRangeAt(current + 1, total);
                    }
                    else
                    {
                        _tcs.SetResult(null);
                    }
                });
            }

            public Task PrintPrimeNumersCount()
            {
                PrintPrimeNumbersRangeAt(0, 10);

                return _tcs.Task;
            }
        }


        private static async void PrimeNumbersRanges()
        {
            //int step = 100000;
            //for (int i = 0; i < 10; i++)
            //{
            //    int count = GetPrimesCount(i * step + 2, step);
            //    Console.WriteLine($"[{i * step, 8}; {(i + 1) * step - 1, 8}] => {count}");
            //}

            PrimeNumbersStateMachine psm = new PrimeNumbersStateMachine();

            await psm.PrintPrimeNumersCount();

            Console.WriteLine(">>method ended");
        }
        // ====================================================================================================


        private static async void TaskCompletionSourceTest()
        {
            var tcs = new TaskCompletionSource<int>();

            Func<Task<int>> task = () =>
            {
                var timer = new System.Timers.Timer();
                timer.Interval = 3000;
                timer.Elapsed += (o, e) => { tcs.SetResult(42); };
                timer.Start();

                return tcs.Task;
            };

            int result = await task();
            Console.WriteLine($"result = {result}");

            Console.WriteLine(">>method ended");
        }





        private static async void PrimeNumbersTaskTest()
        {
            int rangeFrom = 2;
            int rangeTo = 1000000;

            var task = Task.Run(() => ParallelEnumerable.Range(rangeFrom, rangeTo).Count(n =>
                Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i != 0)));

            //var awaiter = task.GetAwaiter();

            //awaiter.OnCompleted(() => 
            //{
            //    int primeCount = awaiter.GetResult();
            //    Console.WriteLine($"Prime count [{rangeFrom}; {rangeTo}] == {primeCount}");

            //});

            int primeCount = await task;
            Console.WriteLine($"Prime count [{rangeFrom}; {rangeTo}] == {primeCount}");

            Console.WriteLine(">>method ended");
        }

        // ========================================================================================
        private static void TaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Task.Run(() =>
            {
                throw new Exception("Unobserved");
            });

            Thread.Sleep(1000);

            Console.WriteLine(">>>> Before collection");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Console.WriteLine(">>>> After collection");
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine(">>>> TaskScheduler_UnobservedTaskException");
            Console.WriteLine(e.Exception);
        }
        // ========================================================================================

        private static void MultithreadingSamples()
        {
            for (int i = 0; i < 20; i++)
            {
                int temp = i;
                //new Thread(() => { Console.Write(i); }).Start();
                //ThreadPool.QueueUserWorkItem((o) => { Console.Write(i); });
                //Task.Factory.StartNew(() => { Console.Write($"{temp} "); }/*, TaskCreationOptions.LongRunning*/);
            }


            // Thread is foreground by default
            //Thread thread = new Thread(() => { Thread.Sleep(5000); Console.Write("Waiting in thread..."); Console.ReadLine(); });
            //thread.IsBackground = true;
            //thread.Start();

            // Task are always background, despite of TaskCreationOptions.LongRunning option
            //Task.Factory.StartNew(() => { Thread.Sleep(5000); Console.Write("Waiting in thread..."); Console.ReadLine(); }, TaskCreationOptions.LongRunning);


            var signal = new ManualResetEvent(false);


            Thread thread = new Thread(() =>
            {
                Console.WriteLine("Thread started");
                Thread.Sleep(1000);
                Console.WriteLine("Waiting for signal...");
                signal.WaitOne();
                Console.WriteLine("Signal recieved");
            });

            thread.Start();
            Console.WriteLine("3s till signal");
            Thread.Sleep(3000);
            Console.WriteLine("signalling");

            ThreadPool.GetAvailableThreads(out int workers, out int ports);
            Console.WriteLine($"{workers} - {ports}");

            signal.Set();
        }


        private static void TaskCombinators()
        {
            Func<Task<int>> t1 = async () => { await Task.Delay(2500); return 1; };
            Func<Task<int>> t2 = async () => { await Task.Delay(2500); return 2; };
            Func<Task<int>> t3 = async () => { await Task.Delay(3000); return 3; };

            var result = Task.WhenAll(t1(), t2(), t3()).Result;

            Console.WriteLine(result[2]);
        }

        private static void TypeConverters()
        {
            TypeConverter conv = TypeDescriptor.GetConverter(typeof(Color));

            const string colorName = "LightBlue";
            var color = (Color)conv.ConvertFromString(colorName);

            bool isEqual = EqualityComparer<Color>.Default.Equals(color, Color.LightBlue);

            Console.WriteLine($"{colorName} = ({color.R}, {color.G}, {color.B}); Equal={isEqual}");
        }

        public static void WordCount()
        {
            Regex word = new Regex(@"\b(\w|[-])+\b");

            string s = "asda fdsg vbgfghhg ertretert   aass-ffff  trtyt  vdfd werwrwer CCCC";

            var matches = word.Matches(s);

            Console.WriteLine($"Words: { matches.Count}| {String.Join(", ", matches.OfType<Match>().Select(m => m.Groups[0]))}");
        }

        public static void TestValueTypeInterfaceBoxing()
        {
            var s = new AddStruct();
            s.Print("Init");

            s.Add(10);
            s.Print("s.Add(10)");

            ((IAdd)s).Add(10);
            s.Print("((IAdd)s).Add(10)");

            AddStruct.Add(ref s, 10);
            s.Print("AddStruct.Add(s, 10)");

            AddStruct.AddConstrained(ref s, 10);
            s.Print("AddStruct.AddConstrained(ref s, 10)");
        }

        public static void TestCultureInfo()
        {
            CultureInfo ci = CultureInfo.GetCultureInfo("ru-RU");

            //Console.WriteLine(5.ToString("C", ci));

            string s = DateTime.Now.ToString("o");

            Console.WriteLine(s);
        }

        [Flags]
        public enum BorderSides { Left = 1, Right = 2, Top = 4, Bottom = 8, LeftRight = Left | Right }

        public static void TestEnums()
        {
            foreach (Enum value in Enum.GetValues(typeof(BorderSides)))
            {
                Console.WriteLine(value);

            }
        }

        public static void SystemNumerics()
        {
            System.Numerics.Vector2 v1 = new System.Numerics.Vector2(1, 1);

        }

        public static void ClosuresTest()
        {
            int v1 = 1;
            Action act1 = () => Console.WriteLine($"act1 : {v1}");
            v1 = 42;
            act1();
            //act1: 42

            List<Action> actList2 = new List<Action>();
            Console.Write("for : ");
            for (int i2 = 0; i2 < 5; i2++)
            {
                actList2.Add(() => Console.Write($"{i2} "));
            }
            actList2.ForEach(a => a());
            Console.WriteLine();
            //for : 5 5 5 5 5


            List<Action> actList3 = new List<Action>();
            Console.Write("foreach : ");
            foreach (int i3 in Enumerable.Range(0, 5))
            {
                actList3.Add(() => Console.Write($"{i3} "));
            }
            actList3.ForEach(a => a());
            Console.WriteLine();
            //foreach : 0 1 2 3 4
        }




        public static void DelegatesTest()
        {
            Delegates d = new Delegates();

            d.Do();
            d.Actions();
        }

        public static void AnonType()
        {
            var val = 111;
            var anon = new { Num = 5, val, 44.ToString().Length };

            Console.WriteLine($"{anon.Num} : {anon.Length} : {anon.val}");
        }

        public static void TristateBoolExample()
        {
            TristateBool var = TristateBool.Undefined;

            if (var)
                Console.WriteLine("True");
            else if (!var)
                Console.WriteLine("False");
            else
                Console.WriteLine("Undefined");
        }

        static void OverrideBool()
        {
            BoolOverride odd = new BoolOverride(1);
            BoolOverride even = new BoolOverride(2);

            if (even)
            {
                Console.WriteLine("even condition true");
            }
            else
            {
                Console.WriteLine("even condition false");
            }

            Console.WriteLine(odd ? "odd condition true" : "odd condition false");
            Console.WriteLine(!even ? "!even condition true" : "!even condition false");
            Console.WriteLine(!odd ? "!odd condition true" : "!odd condition false");

            Console.WriteLine(even.IsTrue ? "even.IsTrue condition true" : "even.IsTrue condition false");
        }

        static void Dump()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var dumper = new AssemblyDumper(path);
            dumper.Dump(Console.Out);
        }
    }
}
