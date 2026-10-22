using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Services;

public class ImageUploadService(IWebHostEnvironment environment)
{
    public void Validate(IFormFile? image, ModelStateDictionary modelState)
    {
        if (image == null) return;
        if (image.Length == 0 || image.Length > 5 * 1024 * 1024)
        {
            modelState.AddModelError("imageFile", "Vui lòng chọn ảnh có dung lượng từ 1 byte đến 5 MB.");
            return;
        }
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        using var stream = image.OpenReadStream();
        Span<byte> header = stackalloc byte[12];
        var count = stream.ReadAtLeast(header, header.Length, throwOnEndOfStream: false);
        var valid = extension switch
        {
            ".jpg" or ".jpeg" => count >= 3 && header[0] == 0xff && header[1] == 0xd8 && header[2] == 0xff,
            ".png" => count >= 8 && header[..8].SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            ".gif" => count >= 6 && (header[..6].SequenceEqual("GIF87a"u8) || header[..6].SequenceEqual("GIF89a"u8)),
            ".webp" => count >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8),
            _ => false
        };
        if (!valid) modelState.AddModelError("imageFile", "Chỉ hỗ trợ tệp ảnh JPG, PNG, WEBP hoặc GIF hợp lệ.");
    }

    public string Save(IFormFile image, string folder)
    {
        var directory = Path.Combine(environment.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(directory);
        var name = $"{Guid.NewGuid():N}{Path.GetExtension(image.FileName).ToLowerInvariant()}";
        using var output = new FileStream(Path.Combine(directory, name), FileMode.CreateNew);
        image.CopyTo(output);
        return $"/uploads/{folder}/{name}";
    }
}
