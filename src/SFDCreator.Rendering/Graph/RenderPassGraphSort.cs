namespace SFDCreator.Rendering.Graph;

public static class RenderPassGraphSort
{
    public static IReadOnlyList<PassNode> Sort(IReadOnlyList<PassNode> passes)
    {
        var indegree = new Dictionary<PassNode, int>();
        var dependents = new Dictionary<PassNode, List<PassNode>>();

        foreach (var pass in passes)
        {
            indegree[pass] = 0;
            dependents[pass] = new List<PassNode>();
        }

        foreach (var producer in passes)
        {
            foreach (var consumer in passes)
            {
                if (ReferenceEquals(producer, consumer))
                {
                    continue;
                }

                if (consumer.Reads.Any(resource => producer.Writes.Contains(resource)))
                {
                    dependents[producer].Add(consumer);
                    indegree[consumer]++;
                }
            }
        }

        var ready = new Queue<PassNode>(passes.Where(pass => indegree[pass] == 0));
        var result = new List<PassNode>();

        while (ready.Count > 0)
        {
            var pass = ready.Dequeue();
            result.Add(pass);

            foreach (var dependent in dependents[pass])
            {
                indegree[dependent]--;
                if (indegree[dependent] == 0)
                {
                    ready.Enqueue(dependent);
                }
            }
        }

        if (result.Count != passes.Count)
        {
            throw new InvalidOperationException("The render graph contains a cycle between passes.");
        }

        return result;
    }
}
