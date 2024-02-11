using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultimediaLibrary.Controllers;
using MultimediaLibrary.Data;
using MultimediaLibrary.Models;
using MultimediaLibrary.Models.TransferModels;
using MultimediaLibrary.Services.AuthService;
using BlurHashSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using System.Text;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace MultimediaLibrary.Services.UploadService
{
    public class UploadService : IUploadService
    {
        private readonly DatabaseContext _db;
        private readonly IAuthService _authService;
        private readonly MultimediaLibrarySettings _settings;
        

        public UploadService(DatabaseContext db, IAuthService authService, IOptions<MultimediaLibrarySettings> settings)
        {
            _db = db;
            _authService = authService;
            _settings = settings.Value;
        }


        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "No file was uploaded";
            }

            string filePath = Path.Combine(_settings.DirectoryPaths.Original, file.FileName);

            if (!Directory.Exists(_settings.DirectoryPaths.Original))
            {
                Directory.CreateDirectory(_settings.DirectoryPaths.Original);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return filePath; 
        }

        public async Task<string> CancelUpload(string uuid)
        {
            string filePath = Path.Combine(_settings.DirectoryPaths.Original, uuid);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return await Task.FromResult("success");
            }
            return await Task.FromResult("file does not exist");
        }

        public async Task<string> SaveUpload(SaveUploadData data)
        {
            ulong? userId = _authService.GetCurrentUserId();
            if (userId == null)
                return await Task.FromResult("user not logged in");

            string filePath = Path.Combine(_settings.DirectoryPaths.Original, data.Uuid);
            if (File.Exists(filePath))
            {
                ImageMetadata exif = Image.Identify(filePath).Metadata;
                StringBuilder exifdata = new StringBuilder();
                for (int i = 0; i < exif.ExifProfile?.Values.Count; i++)
                {
                    var tag = exif.ExifProfile.Values[i].Tag;
                    if (tag.Equals("MakerNote"))
                        continue;
                    var value = exif.ExifProfile.Values[i].GetValue();
                    string valueString;
                    if (value is Array array)
                    {
                        valueString = $"[{string.Join(",", array.Cast<object>())}]";
                    }
                    else
                        valueString = value.ToString();
                    exifdata.Append($"{{\"{tag}\":\"{valueString}\"}}");
                    if (i < exif.ExifProfile.Values.Count - 1)
                        exifdata.Append(",");
                }
                var blurTime = DateTime.Now;
                string blurhash = CreateBlurhash(filePath);
                Console.WriteLine($"Time to create blurhash: {DateTime.Now - blurTime}");
                var media = new Media();
                media.UserId = userId.Value;
                media.MediaUuid = data.Uuid;
                media.Title = data.Title;
                media.Description = data.Description;
                media.DateUpdated = DateTime.Now;
                media.DateCreated = DateTime.Now;
                media.Blurhash = blurhash;
                IExifValue<Number> width = null;
                media.Width = data.Width ?? 0;
                media.Height = data.Height ?? 0;
                media.Size = data.Size ?? 0;
                media.Exif = exifdata.ToString();
                media.Access = data.Access == "Public" ? 1 : data.Access == "Private" ? 0 : -1;
                _db.Media.Add(media);
                _db.SaveChanges();
                var start = DateTime.Now;
                Task.Run(() => CreateResponsiveImagesAsync(data.Uuid));
                Console.WriteLine($"Time to create responsive images: {DateTime.Now - start}");
                return await Task.FromResult("success");
            }
            return await Task.FromResult("file does not exist");
        }

        private string CreateBlurhash(string filePath)
        {
            string blurhash = null;
            using (var image = Image.Load<Rgb24>(filePath))
            {
                image.Mutate(x => x.Resize(0, 50));
                byte[] pixelData = new byte[image.Width*image.Height*3];
                image.CopyPixelDataTo(pixelData);
                blurhash = CoreBlurHashEncoder.Encode(4, 3, image.Width, image.Height, new ReadOnlySpan<byte>(pixelData), image.Width*3, PixelFormat.RGB888);
            }
            return blurhash;
        }

        private async Task CreateResponsiveImagesAsync(string uuid)
        {
            try
            {
                string filePath = Path.Combine(_settings.DirectoryPaths.Original, uuid);
                

                using (var image = Image.Load(filePath))
                {
                    if (Math.Min(image.Width, image.Height) > 2160)
                    {
                        if (!Directory.Exists(_settings.DirectoryPaths.Uhd))
                            Directory.CreateDirectory(_settings.DirectoryPaths.Uhd);
                        using (var uhd = image.Clone(i => i.Resize(image.Width < image.Height ? 2160 : 0, image.Height <= image.Width ? 2160 : 0)))
                        {
                            uhd.SaveAsJpeg(_settings.DirectoryPaths.Uhd + "\\" + uuid, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                            {
                                Quality = 70
                            });
                        }
                    }
                    if (Math.Min(image.Width, image.Height) > 1080)
                    {
                        if (!Directory.Exists(_settings.DirectoryPaths.Fhd))
                            Directory.CreateDirectory(_settings.DirectoryPaths.Fhd);
                        using (var fhd = image.Clone(i => i.Resize(image.Width < image.Height ? 1080 : 0, image.Height <= image.Width ? 1080 : 0)))
                        {
                            fhd.SaveAsJpeg(_settings.DirectoryPaths.Fhd + "\\" + uuid, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                            {
                                Quality = 70
                            });
                        }
                    }
                    if (Math.Min(image.Width, image.Height) > 720)
                    {
                        if (!Directory.Exists(_settings.DirectoryPaths.Hd))
                            Directory.CreateDirectory(_settings.DirectoryPaths.Hd);
                        using (var hd = image.Clone(i => i.Resize(image.Width < image.Height ? 720 : 0, image.Height <= image.Width ? 720 : 0)))
                        {
                            hd.SaveAsJpeg(_settings.DirectoryPaths.Hd + "\\" + uuid, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                            {
                                Quality = 70
                            });
                        }
                    }
                    if (Math.Min(image.Width, image.Height) > 480)
                    {
                        if (!Directory.Exists(_settings.DirectoryPaths.Sd))
                            Directory.CreateDirectory(_settings.DirectoryPaths.Sd);
                        using (var sd = image.Clone(i => i.Resize(image.Width < image.Height ? 480 : 0, image.Height <= image.Width ? 480 : 0)))
                        {
                            sd.SaveAsJpeg(_settings.DirectoryPaths.Sd + "\\" + uuid, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                            {
                                Quality = 70
                            });
                        }
                    }
                    if (Math.Min(image.Width, image.Height) > 150)
                    {
                        if (!Directory.Exists(_settings.DirectoryPaths.Thumbnail))
                            Directory.CreateDirectory(_settings.DirectoryPaths.Thumbnail);
                        using (var sd = image.Clone(i => i.Resize(image.Width < image.Height ? 150 : 0, image.Height <= image.Width ? 150 : 0)))
                        {
                            sd.SaveAsJpeg(_settings.DirectoryPaths.Thumbnail + "\\" + uuid, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                            {
                                Quality = 70
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while creating responsive images for {uuid}  error msg: {e.Message}");
            }
        }
    }
}
