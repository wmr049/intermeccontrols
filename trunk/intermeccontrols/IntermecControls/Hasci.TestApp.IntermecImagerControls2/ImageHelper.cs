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
    public static bool saveAsJpg(string sFileIn, string sFileOut){
        var filestream = System.IO.File.Open(sFileIn, System.IO.FileMode.Open);
        OpenNETCF.Drawing.Imaging.StreamOnFile sof;
        sof = new StreamOnFile(filestream);
        IImage image;
        ImageInfo info;
        System.Drawing.Bitmap _image;
        IBitmapImage thumbnail;            

        bool bRet = false;
        GetFactory().CreateImageFromStream(sof, out image);
        try
        {
            image.GetImageInfo(out info);
            GetFactory().CreateBitmapFromImage(image, (uint)info.Width, (uint)info.Height, info.PixelFormat, InterpolationHint.InterpolationHintDefault, out thumbnail);
            _image = ImageUtils.IBitmapImageToBitmap(thumbnail);
            _image.Save(sFileOut, ImageFormat.Jpeg);
            bRet = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Exception in saveAsJpg: " + ex.Message);
            bRet = false;
        }
        finally
        {
            Marshal.ReleaseComObject(image);
        }
        return bRet;
    }
    public static string dumpConditioningValues(Intermec.DataCollection.ImageConditioning imgCond)
    {
        string sReturn="";
        sReturn += "##############################################" + "\n";
        sReturn += "Brightness: " + imgCond.Brightness.ToString() + "\n";
        sReturn += "ColorMode: " + imgCond.ColorMode.ToString() + "\n";
        sReturn += "ColorModeBrightnessThreshold: " + imgCond.ColorModeBrightnessThreshold.ToString() + "\n";
        sReturn += "ContrastEnhancement: " + imgCond.ContrastEnhancement.ToString() + "\n";
        sReturn += "CurrentImageConditioningVersion: " + imgCond.CurrentImageConditioningVersion.ToString() + "\n";
        sReturn += "ImageLightingCorrection: " + imgCond.ImageLightingCorrection.ToString() + "\n";
        sReturn += "ImageRotation: " + imgCond.ImageRotation.ToString() + "\n";
        sReturn += "NoiseReduction: " + imgCond.NoiseReduction.ToString() + "\n";
        sReturn += "OutputCompression: " + imgCond.OutputCompression.ToString() + "\n";
        sReturn += "OutputCompressionQuality: " + imgCond.OutputCompressionQuality.ToString() + "\n";
        sReturn += "Subsampling: " + imgCond.Subsampling.ToString() + "\n";
        sReturn += "TextEnhancement: " + imgCond.TextEnhancement.ToString() + "\n";
        sReturn += "##############################################" + "\n";

        return sReturn;
    }
}
