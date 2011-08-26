using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenNETCF.Drawing;
using OpenNETCF.Drawing.Imaging;
using System.Runtime.InteropServices;

public static class ImageHelper
{
    private static ImagingFactory m_factory;

    private static ImagingFactory GetFactory()
    {
        if (m_factory == null)
        {
            m_factory = new ImagingFactory();
        }

        return m_factory;
    }

    public static Bitmap CreateClip(StreamOnFile sof, int x, int y, int width, int height)
    {
        IBitmapImage original = null;
        IImage image = null;
        ImageInfo info;

        GetFactory().CreateImageFromStream(sof, out image);
        try
        {
            image.GetImageInfo(out info);

            GetFactory().CreateBitmapFromImage(image, info.Width, info.Height,
                info.PixelFormat, InterpolationHint.InterpolationHintDefault, out original);

            try
            {
                var ops = (IBasicBitmapOps)original;
                IBitmapImage clip = null;

                try
                {
                    var rect = new RECT(x, y, x + width, y + height);
                    ops.Clone(rect, out clip, true);

                    return ImageUtils.IBitmapImageToBitmap(clip);
                }
                finally
                {
                    Marshal.ReleaseComObject(clip);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(original);
            }
        }
        finally
        {
            Marshal.ReleaseComObject(image);
        }
    }

    public static Bitmap CreateThumbnail(StreamOnFile sof, int width, int height)
    {
        IBitmapImage thumbnail;            
        IImage image;
        ImageInfo info;

        GetFactory().CreateImageFromStream(sof, out image);
        try
        {
            image.GetImageInfo(out info);

            GetFactory().CreateBitmapFromImage(image, (uint)width, (uint)height, 
                info.PixelFormat, InterpolationHint.InterpolationHintDefault, out thumbnail);
            try
            {
                return ImageUtils.IBitmapImageToBitmap(thumbnail);
            }
            finally
            {
                Marshal.ReleaseComObject(thumbnail);
            }
        }
        finally
        {
            Marshal.ReleaseComObject(image);
        }
    }

    public static Size GetRawImageSize(StreamOnFile sof)
    {
        IImage image;
        ImageInfo info;

        GetFactory().CreateImageFromStream(sof, out image);
        try
        {
            image.GetImageInfo(out info);

            return new Size((int)info.Width, (int)info.Height);
        }
        finally
        {
            Marshal.ReleaseComObject(image);
        }
    }
}
