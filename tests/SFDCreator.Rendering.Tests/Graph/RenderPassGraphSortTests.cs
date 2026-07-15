using SFDCreator.Rendering.Graph;

namespace SFDCreator.Rendering.Tests.Graph;

public sealed class RenderPassGraphSortTests
{
    [Fact]
    public void Sort_OrdersProducerBeforeConsumer()
    {
        var resource = new object();
        var producer = new PassNode("Producer", Array.Empty<object>(), new[] { resource });
        var consumer = new PassNode("Consumer", new[] { resource }, Array.Empty<object>());

        var result = RenderPassGraphSort.Sort(new[] { consumer, producer });

        Assert.Equal(new[] { producer, consumer }, result);
    }

    [Fact]
    public void Sort_PreservesOriginalOrderForIndependentPasses()
    {
        var a = new PassNode("A", Array.Empty<object>(), Array.Empty<object>());
        var b = new PassNode("B", Array.Empty<object>(), Array.Empty<object>());

        var result = RenderPassGraphSort.Sort(new[] { a, b });

        Assert.Equal(new[] { a, b }, result);
    }

    [Fact]
    public void Sort_ThrowsOnCycle()
    {
        var resourceA = new object();
        var resourceB = new object();
        var first = new PassNode("First", new[] { resourceB }, new[] { resourceA });
        var second = new PassNode("Second", new[] { resourceA }, new[] { resourceB });

        Assert.Throws<InvalidOperationException>(() => RenderPassGraphSort.Sort(new[] { first, second }));
    }

    [Fact]
    public void Sort_HandlesChainOfThreePasses()
    {
        var resource1 = new object();
        var resource2 = new object();
        var passA = new PassNode("A", Array.Empty<object>(), new[] { resource1 });
        var passB = new PassNode("B", new[] { resource1 }, new[] { resource2 });
        var passC = new PassNode("C", new[] { resource2 }, Array.Empty<object>());

        var result = RenderPassGraphSort.Sort(new[] { passC, passA, passB });

        Assert.Equal(new[] { passA, passB, passC }, result);
    }
}
