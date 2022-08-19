using Android.Graphics;
using Android.Widget;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace FuelMonitor.Utils
{
    public static class MediaPickerUtils
    {
        public static async Task<MediaPickerResult<T>> CaptureImageWithCamera<T>
            (string title)
             where T : class
        {
            return await PickPhoto<T>(MediaPicker.CapturePhotoAsync, title);
        }

        public static async Task<MediaPickerResult<T>> PickImageFromFile<T>
            (string title)
             where T : class
        {
            return await PickPhoto<T>(MediaPicker.PickPhotoAsync, title);
        }

        private static async Task<MediaPickerResult<T>> PickPhoto<T>
            (Func<MediaPickerOptions, Task<FileResult>> mediaPickerFunction, string title = "Capture Image")
             where T : class
        {
            var result = new MediaPickerResult<T>();
            try
            {
                var options = new MediaPickerOptions { Title = title };

                var photoResult = await mediaPickerFunction(options);

                if (photoResult != null)
                {
                    using (var stream = await photoResult.OpenReadAsync())
                    {
                        result.Result = await Convert<T>(stream);
                    }

                    result.Success = true;

                    return result;
                }
                result.ErrorMessage = "cancelled by user";
            }
            catch (FeatureNotSupportedException)
            {
                result.ErrorMessage = "Feature is not supported on the device";
            }
            catch (PermissionException)
            {
                result.ErrorMessage = "Permissions not granted";
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"CapturePhotoAsync THREW: {ex.Message}";
            }

            return result;
        }

        private static async Task<T> Convert<T>(Stream stream) where T : class
        {
            if (typeof(T) == typeof(Bitmap))
            {
                var image = await BitmapFactory.DecodeStreamAsync(stream);
                if (image.Height > image.Width)
                {
                    var mat = new Matrix();
                    mat.PostRotate(-90);
                    image = Bitmap.CreateBitmap(image, 0, 0, image.Width, image.Height, mat, true);
                }

                return image as T;
            }

            throw new NotImplementedException();
        }
    }

    public class MediaPickerResult<T>
    {
        public bool Success { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}