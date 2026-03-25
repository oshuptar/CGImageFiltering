using GCImageFiltering.Core.Quantization.Pixel;

namespace GCImageFiltering.Core.Quantization.DataStructure;

public class Octree
{
    public Node? Root { get; set; } = null;
    public int MaxLeaves { get; set; }

    public int LeafCount { get; set; } = 0;

    public List<List<Node>>? InnerChildren { get; set; } = null;

    public Octree(int maxLeaves)
    {
        MaxLeaves = maxLeaves;
    }

    public void Insert(RgbaPixel pixel)
    {
        throw new NotImplementedException();
    }

    public void Reduce()
    {
        throw new NotImplementedException();   
    }

    public RgbaPixel FindColor(RgbaPixel pixel)
    {
        throw new NotImplementedException();  
    }
}