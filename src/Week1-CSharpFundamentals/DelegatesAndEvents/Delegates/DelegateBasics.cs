namespace DelegatesAndEvents.Delegates
{
    public class DelegateBasics
    {
        // Define a delegate type that takes an int and returns void
        public delegate void MyDelegate(int number);

        // A method that matches the delegate signature
        public void PrintNumber(int number)
        {
            Console.WriteLine($"The number is: {number}");
        }

        // Another method that matches the delegate signature
        public void SquareNumber(int number)
        {
            Console.WriteLine($"The square of the number is: {number * number}");
        }

        public void Run()
        {
            // Create an instance of the delegate and assign it to the PrintNumber method
            MyDelegate myDelegate = PrintNumber;

            // Invoke the delegate with a number
            myDelegate(5); // Output: The number is: 5

            // Reassign the delegate to the SquareNumber method
            myDelegate = SquareNumber;

            // Invoke the delegate again with a number
            myDelegate(5); // Output: The square of the number is: 25

            // Combine delegates to call both methods in sequence
            MyDelegate combinedDelegate = (MyDelegate)Delegate.Combine(
                new MyDelegate(PrintNumber),
                new MyDelegate(SquareNumber)
            );

            // Invoke the combined delegate
            combinedDelegate(5);
            // Output:
            // The number is: 5
            // The square of the number is: 25

            // Anonymous method (pre-lambda syntax using the 'delegate' keyword)
            MyDelegate anonymousDelegate = delegate(int n)
            {
                Console.WriteLine($"Anonymous method received: {n}");
            };

            anonymousDelegate(7); // Output: Anonymous method received: 7

            // Equivalent lambda — more concise than anonymous method
            MyDelegate lambdaDelegate = n => Console.WriteLine($"Lambda received: {n}");
            lambdaDelegate(7); // Output: Lambda received: 7
        }
    }
}