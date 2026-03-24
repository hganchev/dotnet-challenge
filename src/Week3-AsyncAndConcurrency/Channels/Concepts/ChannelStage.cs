using System.Threading.Channels;

namespace Channels.Concepts;

/// <summary>
/// A reusable pipeline stage that reads from an input channel, transforms each
/// item with <paramref name="transform"/>, and writes results to an output channel.
/// Call <see cref="RunAsync"/> to start processing; the output channel is
/// completed automatically once the input channel is drained.
/// </summary>
public sealed class ChannelStage<TIn, TOut>
{
    private readonly ChannelReader<TIn> _input;
    private readonly ChannelWriter<TOut> _output;
    private readonly Func<TIn, CancellationToken, ValueTask<TOut>> _transform;
    private readonly string _name;

    public ChannelStage(
        ChannelReader<TIn> input,
        ChannelWriter<TOut> output,
        Func<TIn, CancellationToken, ValueTask<TOut>> transform,
        string name = "Stage")
    {
        _input = input;
        _output = output;
        _transform = transform;
        _name = name;
    }

    /// <summary>
    /// Processes all items from the input channel until it is completed or
    /// <paramref name="ct"/> is cancelled.  Completes the output writer when done.
    /// </summary>
    public async Task RunAsync(CancellationToken ct = default)
    {
        try
        {
            await foreach (var item in _input.ReadAllAsync(ct).ConfigureAwait(false))
            {
                var result = await _transform(item, ct).ConfigureAwait(false);
                await _output.WriteAsync(result, ct).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"  [{_name}] cancelled");
        }
        finally
        {
            _output.Complete();
        }
    }
}
