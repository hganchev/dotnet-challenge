namespace Collections.Concepts;

/// <summary>
/// Demonstrates different access patterns and when to use Stack/Queue vs List.
/// </summary>
public static class StackQueueLinkedListExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Stack, Queue, and LinkedList Semantics ---");

        // Stack: LIFO
        Console.WriteLine("Stack<T> (LIFO — Last In, First Out):");
        var stack = new Stack<string>();
        stack.Push("First");
        stack.Push("Second");
        stack.Push("Third");
        Console.WriteLine($"  Peek: {stack.Peek()}");
        while (stack.Count > 0)
            Console.WriteLine($"  Pop: {stack.Pop()}");

        // Queue: FIFO
        Console.WriteLine("\nQueue<T> (FIFO — First In, First Out):");
        var queue = new Queue<string>();
        queue.Enqueue("First");
        queue.Enqueue("Second");
        queue.Enqueue("Third");
        Console.WriteLine($"  Peek: {queue.Peek()}");
        while (queue.Count > 0)
            Console.WriteLine($"  Dequeue: {queue.Dequeue()}");

        // LinkedList: arbitrary traversal
        Console.WriteLine("\nLinkedList<T> (arbitrary access by node):");
        var linked = new LinkedList<string>();
        linked.AddLast("A");
        linked.AddLast("B");
        linked.AddLast("C");
        Console.WriteLine($"  All nodes: {string.Join(", ", linked)}");

        var nodeB = linked.Find("B");
        if (nodeB is not null)
        {
            linked.AddBefore(nodeB, "B-Before");
            linked.AddAfter(nodeB, "B-After");
        }
        Console.WriteLine($"  After insertions: {string.Join(", ", linked)}");
        Console.WriteLine("\nNote: LinkedList is useful for frequent mid-list insertions (with node ref)");
        Console.WriteLine("      Otherwise, List<T> is faster due to better cache locality!");
    }
}
