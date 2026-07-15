namespace SFDCreator.Rendering.Graph;

public sealed class PassNode
{
    public PassNode(string name, IReadOnlyList<object> reads, IReadOnlyList<object> writes)
    {
        Name = name;
        Reads = reads;
        Writes = writes;
    }

    public string Name { get; }

    public IReadOnlyList<object> Reads { get; }

    public IReadOnlyList<object> Writes { get; }
}
