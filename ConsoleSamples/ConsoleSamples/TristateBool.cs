namespace ConsoleSamples
{
    public class TristateBool
    {
        private int _data;

        public static readonly TristateBool False = new TristateBool(-1);
        public static readonly TristateBool Undefined = new TristateBool(0);
        public static readonly TristateBool True = new TristateBool(1);


        private TristateBool(int data)
        {
            _data = data;
        }

        public static bool operator true(TristateBool value) => value._data == True._data;
        public static bool operator false(TristateBool value) => value._data == False._data;
        public static TristateBool operator !(TristateBool value) => value._data == Undefined._data ? Undefined : value._data == True._data ? False : True;
    }
}
