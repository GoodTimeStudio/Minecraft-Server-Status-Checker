using Lumia.Imaging;
using Lumia.Imaging.Transforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Minecraft_Server_Status_Checker.Status
{

    //see http://wiki.vg/Mojang_API or http://www.mcbbs.net/thread-592297-1-1.html
    public class SkinHelper
    {

        public static readonly string uuidUri = "https://api.mojang.com/users/profiles/minecraft/<username>?at=<timestamp>";
        public static readonly string profileUri = "https://sessionserver.mojang.com/session/minecraft/profile/<uuid>";

        public static async Task<Stream> GetPlayerSkinAsync(string playerName)
        {
            string uuid = await GetPlayerUUIDAsync(playerName);
            string uri = profileUri.Replace("<uuid>", uuid);

            using (HttpClient client = new HttpClient())
            {
                string profile = null;
                try
                {
                    HttpResponseMessage profileResponse = await client.GetAsync(uri);
                    profileResponse.EnsureSuccessStatusCode();
                    profile = await profileResponse.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException)
                {
                    client.Dispose();
                    return null;
                }

                string skinUri = null;
                if (!string.IsNullOrEmpty(profile))
                {
                   try
                    {
                        JsonObject obj = JsonObject.Parse(profile);
                        JsonArray array = obj["properties"].GetArray();
                        JsonObject subObj = array.GetObjectAt(0);
                        string value = Base64Helper.DecodeBase64(subObj["value"].GetString());

                        obj = JsonObject.Parse(value);
                        skinUri = obj["textures"].GetObject()["SKIN"].GetObject()["url"].GetString();
                    }
                    catch (COMException)
                    {
#if DEBUG
                        Debug.WriteLine("Profile json parse failed");
#endif
                        client.Dispose();
                        return null;
                    }
                    catch (KeyNotFoundException)
                    {
                        client.Dispose();
                        return null;
                    }                 
                }

                if (!string.IsNullOrEmpty(skinUri))
                {
                    try
                    {
                        HttpResponseMessage skinResponse = await client.GetAsync(skinUri);
                        skinResponse.EnsureSuccessStatusCode();
                        Stream stream = await skinResponse.Content.ReadAsStreamAsync();
                        client.Dispose();
                        return stream;
                    }
                    catch (HttpRequestException)
                    {
                        client.Dispose();
                        return null;
                    }
                    
                }

                client.Dispose();
                return null;
            }
        }

        public static async Task<string> GetPlayerUUIDAsync(string playerName)
        {
            //Convert to UNIX timestamp
            DateTime begin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            string timestamp = Convert.ToString(Convert.ToInt64((DateTime.Now - begin).TotalSeconds));

            string uri = uuidUri.Replace("<username>", playerName);
            uri = uri.Replace("<timestamp>", timestamp);

            using (HttpClient client = new HttpClient())
            {              
                string responseBody = null;
                try
                {
                    HttpResponseMessage response = await client.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException)
                {
                    client.Dispose();
                    return null;
                }
                

                if (!string.IsNullOrEmpty(responseBody))
                {
                    try
                    {
                        JsonObject obj = JsonObject.Parse(responseBody);
                        return obj["id"].GetString();
                    }
                    catch (COMException)
                    {                      
#if DEBUG
                        Debug.WriteLine("UUID json parse failed");
#endif
                        client.Dispose();
                        return null;
                    }
                }

                client.Dispose();
            }

            return null;
        }

        public static async Task<WriteableBitmap> GetPlayerFaceAsync(string playerName)
        {
            Stream skinStream = await GetPlayerSkinAsync(playerName);

            if (skinStream != null)
            {
                IRandomAccessStream stream = skinStream.AsRandomAccessStream();
                StreamImageSource source = new StreamImageSource(skinStream);
                //return await CropImage(stream, new Rect(8, 8, 8, 8), 4);
                var tmp = await CropImageNew(source, new Rect(8, 8, 8, 8));
                return await ResizeBitmap(tmp, 32, 32);
            }

            return null;
        }

        //from https://code.msdn.microsoft.com/windowsapps/CSWin8AppCropBitmap-52fa1ad7
        public static async Task<WriteableBitmap> CropImage(IRandomAccessStream source, Rect cropAera, double scale)
        {
            uint startPointX = (uint) Math.Floor(cropAera.X * scale);
            uint startPointY = (uint) Math.Floor(cropAera.Y * scale);
            uint height = (uint) Math.Floor(cropAera.Height * scale);
            uint width = (uint) Math.Floor(cropAera.Width * scale);

            // Create a decoder from the stream. With the decoder, we can get  
            // the properties of the image. 
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(source);

            // The scaledSize of original image. 
            uint scaledWidth = (uint) Math.Floor(decoder.PixelWidth * scale);
            uint scaledHeight = (uint) Math.Floor(decoder.PixelHeight * scale);

            // Refine the start point and the size.  
            if (startPointX + width > scaledWidth)
            {
                startPointX = scaledWidth - width;
            }

            if (startPointY + height > scaledHeight)
            {
                startPointY = scaledHeight - height;
            }

            // Create cropping BitmapTransform and define the bounds. 
            BitmapTransform transform = new BitmapTransform();
            BitmapBounds bounds = new BitmapBounds();
            bounds.X = startPointX;
            bounds.Y = startPointY;
            bounds.Height = height;
            bounds.Width = width;
            transform.Bounds = bounds;

            transform.ScaledWidth = scaledWidth;
            transform.ScaledHeight = scaledHeight;

            // Get the cropped pixels within the bounds of transform. 
            PixelDataProvider pix = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.ColorManageToSRgb);
            byte[] pixels = pix.DetachPixelData();

            // Stream the bytes into a WriteableBitmap 
            WriteableBitmap cropBmp = new WriteableBitmap((int)width, (int)height);
            Stream pixStream = cropBmp.PixelBuffer.AsStream();
            pixStream.Write(pixels, 0, (int)(width * height * 4));

            source.Dispose();

            return cropBmp;
        }

        public static async Task<WriteableBitmap> CropImageNew(IImageProvider source, Rect cropAera)
        {
            WriteableBitmap ret = new WriteableBitmap((int) cropAera.Width, (int) cropAera.Height);

            using (CropEffect croper = new CropEffect(source, cropAera))
            {
                using (WriteableBitmapRenderer render = new WriteableBitmapRenderer(croper, ret))
                {
                    render.OutputOption = OutputOption.Stretch;
                    await render.RenderAsync();
                }
            }

            return ret;
        }

        public static async Task<WriteableBitmap> ResizeBitmap(WriteableBitmap source, int width, int height)
        {
            AutoResizeConfiguration config = new AutoResizeConfiguration();
            config.ResizeMode = AutoResizeMode.Automatic;
            config.MaxImageSize = new Size(width, height);
            config.MinImageSize = new Size(width, height);

            IBuffer output = await JpegTools.AutoResizeAsync(source.PixelBuffer, config);
            WriteableBitmap ret = new WriteableBitmap(width, height);
            ret.SetSource(output.AsStream().AsRandomAccessStream());
            return ret;
        }
    }
}
