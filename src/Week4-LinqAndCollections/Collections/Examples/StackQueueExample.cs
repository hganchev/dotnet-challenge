namespace Collections.Examples;

/// <summary>
/// Demonstrates Stack<T> and Queue<T> for LIFO and FIFO semantics respectively.
/// Better than List<T> for these access patterns.
/// </summary>
public static class StackQueueExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Stack<T> and Queue<T> ---");

        // Stack: LIFO (Last In, First Out)
        Console.WriteLine("Stack<T> (LIFO):");
        var stack = new Stack<int>();
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        Console.WriteLine($"Pushed 10, 20, 30: Count = {stack.Count}");
        Console.WriteLine($"Peek: {stack.Peek()} (doesn't remove)");

        Console.WriteLine("Popping:");
        while (stack.Count > 0)
        {
            Console.WriteLine($"  Pop: {stack.Pop()}");
        }

        // Queue: FIFO (First In, First Out)
        Console.WriteLine("\nQueue<T> (FIFO):");
        var queue = new Queue<int>();
        queue.Enqueue(10);
        queue.Enqueue(20);
        queue.Enqueue(30);
        Console.WriteLine($"Enqueued 10, 20, 30: Count = {queue.Count}");
        Console.WriteLine($"Peek: {queue.Peek()} (doesn't remove)");

        Console.WriteLine("Dequeuing:");
        while (queue.Count > 0)
        {
            Console.WriteLine($"  Dequeue: {queue.Dequeue()}");
        }

        // Example: Expression parsing with Stack
        Console.WriteLine("\nExpression parsing (parenthesis matching) with Stack:");
        string[] expressions = { "()", "(())", "(()", "())" };
        foreach (var expr in expressions)
        {
            var s = new Stack<char>();
            var balanced = true;
            foreach (var c in expr)
            {
                if (c == '(')
                    s.Push(c);
                else if (c == ')')
                {
                    if (s.Count == 0)
                    {
                        balanced = false;
                        break;
                    }
                    s.Pop();
                }
            }
            balanced = balanced && s.Count == 0;
            Console.WriteLine($"  {expr}: {(balanced ? "Balanced" : "Not balanced")}");
        }
    }
}
