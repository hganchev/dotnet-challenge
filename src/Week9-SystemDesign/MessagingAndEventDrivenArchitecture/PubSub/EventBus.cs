using System.Threading.Channels;

namespace MessagingAndEventDrivenArchitecture.PubSub;

/// <summary>In-memory pub/sub bus built on <see cref="System.Threading.Channels"/>. Each subscriber
/// gets its own channel so every subscriber receives every published event (topic/broadcast
/// semantics), unlike a single shared channel which would give queue semantics (one consumer per message).</summary>
public class EventBus
{
    private readonly List<Channel<Event>> _subscriberChannels = [];

    public ChannelReader<Event> Subscribe()
    {
        var channel = Channel.CreateUnbounded<Event>();
        _subscriberChannels.Add(channel);
        return channel.Reader;
    }

    public async Task PublishAsync(Event evt)
    {
        foreach (var channel in _subscriberChannels)
        {
            await channel.Writer.WriteAsync(evt);
        }
    }

    public void CompleteAll()
    {
        foreach (var channel in _subscriberChannels)
        {
            channel.Writer.Complete();
        }
    }
}
