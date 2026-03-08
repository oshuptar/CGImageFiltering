using System;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CGImageFiltering.App.Buffers;

public sealed class DirectBitmap
{
    public const int BytesPerPixel = 4;
    public PixelFormat PixelFormat { get; }
    public int Width { get; }
    public int Height { get; }
    public int Stride { get; }
    public Vector Dpi { get; }
    public byte[] Pixels { get; set; }
    public WriteableBitmap Bitmap { get; }
    private DirectBitmap(int width, int height, Vector dpi, PixelFormat pixelFormat)
    {
        Width = width;
        Height = height;
        Dpi = dpi;
        Stride = width * BytesPerPixel;
        Pixels = new byte[Stride * height];
        PixelFormat = pixelFormat;
        Bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            Dpi,
            pixelFormat,
            AlphaFormat.Unpremul
        );
    }
    
    public DirectBitmap(int width, int height, Vector dpi, PixelFormat pixelFormat, byte[] pixels)
        : this(width, height, dpi, pixelFormat)
    {
        Pixels = pixels.ToArray();
        UpdateBitmap();
    }

    public static DirectBitmap ToRgba(Bitmap bitmap)
    {
        var size = bitmap.PixelSize;
        var dpi = bitmap.Dpi;
        var directBitmap = new DirectBitmap(size.Width, size.Height, dpi, PixelFormat.Rgba8888);
        directBitmap.ReadFrom(bitmap);
        directBitmap.UpdateBitmap();
        return directBitmap;
    }
    
    private void ReadFrom(Bitmap bitmap)
    {
        using var fb = Bitmap.Lock();
        bitmap.CopyPixels(fb, AlphaFormat.Unpremul);

        int totalBytes = fb.RowBytes * fb.Size.Height;
        byte[] temp = new byte[totalBytes];
        Marshal.Copy(fb.Address, temp, 0, totalBytes);

        for (int y = 0; y < Height; y++)
        {
            Buffer.BlockCopy(
                temp,
                y * fb.RowBytes,
                Pixels,
                y * Stride,
                Stride
            );
        }
    }
        
    public void UpdateBitmap()
    {
        using var fb = Bitmap.Lock();
        for (int y = 0; y < Height; y++)
        {
            IntPtr destRow = fb.Address + y * fb.RowBytes;
            Marshal.Copy(Pixels, y * Stride, destRow, Stride);
        }
    }
}