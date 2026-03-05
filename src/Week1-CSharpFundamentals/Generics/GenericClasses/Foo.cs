namespace Generics.GenericClasses
{
    // A generic class that can hold a value of any type
    public class Foo<T>
    {
        private T _value;

        public Foo(T value)
        {
            _value = value;
        }

        public T GetValue()
        {
            return _value;
        }

        public void SetValue(T value)
        {
            _value = value;
        }
    }
}