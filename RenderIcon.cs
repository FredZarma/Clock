using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

internal static class RenderIcon
{
    [STAThread]
    private static int Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        string outDir = args.Length > 0 ? args[0] : ".";
        Directory.CreateDirectory(outDir);

        int[] sizes = { 16, 24, 32, 48, 64, 128, 256 };
        List<byte[]> pngs = new List<byte[]>();
        for (int i = 0; i < sizes.Length; i++)
        {
            byte[] png = RenderPng(sizes[i]);
            pngs.Add(png);
        }

        string icoPath = Path.Combine(outDir, "ztime.ico");
        WriteIco(icoPath, pngs);
        Console.WriteLine("Wrote " + icoPath);
        return 0;
    }

    private static byte[] RenderPng(int size)
    {
        DrawingVisual dv = new DrawingVisual();
        using (DrawingContext dc = dv.RenderOpen())
        {
            double r = size * 0.22;
            Color face = Color.FromRgb(0x10, 0x12, 0x10);
            Color led = Color.FromRgb(0x3D, 0xFF, 0x8A);
            dc.DrawRoundedRectangle(new SolidColorBrush(face),
                new Pen(new SolidColorBrush(Color.FromRgb(0x2A, 0x3A, 0x2E)), Math.Max(1, size / 32.0)),
                new Rect(0.5, 0.5, size - 1, size - 1), r, r);

            double pad = size * 0.16;
            FormattedText ft = new FormattedText(
                size >= 32 ? "12:44" : "12",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                size * (size >= 32 ? 0.28 : 0.42),
                new SolidColorBrush(led));
            dc.DrawText(ft, new Point((size - ft.Width) / 2.0, (size - ft.Height) / 2.0 - size * 0.02));
        }

        RenderTargetBitmap rtb = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(dv);
        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(rtb));
        using (MemoryStream ms = new MemoryStream())
        {
            encoder.Save(ms);
            return ms.ToArray();
        }
    }

    private static void WriteIco(string path, List<byte[]> pngs)
    {
        using (FileStream fs = File.Create(path))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            bw.Write((ushort)0);
            bw.Write((ushort)1);
            bw.Write((ushort)pngs.Count);
            int offset = 6 + 16 * pngs.Count;
            for (int i = 0; i < pngs.Count; i++)
            {
                int dim = ReadPngSize(pngs[i]);
                bw.Write((byte)(dim >= 256 ? 0 : dim));
                bw.Write((byte)(dim >= 256 ? 0 : dim));
                bw.Write((byte)0);
                bw.Write((byte)0);
                bw.Write((ushort)1);
                bw.Write((ushort)32);
                bw.Write(pngs[i].Length);
                bw.Write(offset);
                offset += pngs[i].Length;
            }
            for (int i = 0; i < pngs.Count; i++)
                bw.Write(pngs[i]);
        }
    }

    private static int ReadPngSize(byte[] png)
    {
        return (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19];
    }
}
