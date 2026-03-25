using GCImageFiltering.Core.Quantization.Pixel;

namespace GCImageFiltering.Core.Quantization.DataStructure;

public class Octree
{
    public Node? Root { get; set; } = null;
    public int MaxLeaves { get; set; }
    public int LeafCount { get; set; } 
    public List<Node>?[] InnerChildren { get; set; } = new List<Node>?[8];

    public Octree(int maxLeaves)
    {
        MaxLeaves = maxLeaves;
        LeafCount = 0;
    }

    public void Insert(RgbaPixel pixel)
    {
        if (Root == null)
            Root = CreateNode(depth: 0);
        AddRecursive(Root, pixel, depth: 0);
        while (LeafCount > MaxLeaves)
            Reduce();
    }

    public void AddRecursive(Node node, RgbaPixel pixel, int depth)
    {
        if (node.IsLeaf)
        {
            node.SumR += pixel.R;
            node.SumG += pixel.G;
            node.SumB += pixel.B;
            node.Count += 1;
        }
        else
        {
            int index = GetIndex(pixel, depth);
            node.Children[index] ??= CreateNode(depth + 1); 
            AddRecursive(node.Children[index]!, pixel, depth + 1);
        }
    }

    public void Reduce()
    {
        int i = InnerChildren.Length - 1;
        for (; i >= 0; i--)
        {
            // Each Inner node has at least one child
            if (InnerChildren[i] is not null && InnerChildren[i]!.Count != 0)
                break;
        }
        if (i < 0)
            throw new InvalidOperationException("No reducible nodes found.");
        Node node = InnerChildren[i]!.Last();
        int removed = 0;
        for (int k = 0; k < node.Children.Length; k++)
        {
            if (node.Children[k] is not null)
            {
                node.SumR += node.Children[k]!.SumR;
                node.SumG += node.Children[k]!.SumG;
                node.SumB += node.Children[k]!.SumB;
                node.Count += node.Children[k]!.Count;
                node.Children[k] = null;
                removed += 1;
            }
        }
        node.IsLeaf = true;
        InnerChildren[i]!.RemoveAt(InnerChildren[i]!.Count - 1);
        LeafCount = LeafCount - removed + 1;
    }

    public RgbaPixel FindColor(RgbaPixel pixel)
    {
        Node? node = Root;
        int depth = 0;
        while (node?.IsLeaf != true)
        {
            int index = GetIndex(pixel, depth);
            node = node?.Children[index];
            depth += 1;
        }
        return new RgbaPixel((byte)(node.SumR / node.Count),
            (byte)(node.SumG / node.Count),
            (byte)(node.SumB / node.Count),
            pixel.A);
    }

    private int GetIndex(RgbaPixel pixel, int depth)
    {
        int bitR = (pixel.R >> (7 - depth)) & 1;
        int bitG = (pixel.G >> (7 - depth)) & 1;
        int bitB = (pixel.B >> (7 - depth)) & 1;
        return 4*bitR + 2*bitG + bitB;
    }
    
    private Node CreateNode(int depth)
    {
        Node node = new Node
        {
            IsLeaf = (depth == 8)
        };
        if (node.IsLeaf)
            LeafCount++;
        else
        {
            if(InnerChildren[depth] == null) InnerChildren[depth] = new List<Node>();
            InnerChildren[depth]!.Add(node);
        }
        return node;
    }
}