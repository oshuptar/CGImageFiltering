namespace GCImageFiltering.Core.Quantization.DataStructure;

public class Node
{
    public bool IsLeaf { get; set; }
    public int SumR { get; set; }
    public int SumG { get; set; }
    public int SumB { get; set; }
    public int Count { get; set; }
    public List<Node> Children { get; set; } = new();
}