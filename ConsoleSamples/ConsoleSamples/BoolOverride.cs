namespace ConsoleSamples
{
    public class BoolOverride
    {
        private int _data;

        public BoolOverride(int data)
        {
            _data = data;
        }

        // Call operator method
        public bool IsTrue => (bool)typeof(BoolOverride).GetMethod("op_Implicit").Invoke(null, new object[] { this });

        public static implicit operator bool(BoolOverride value) => value._data % 2 == 0;
    }
}
