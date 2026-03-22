using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration config)
    {
        var cloudName = config["Cloudinary:CloudName"];
        var apiKey = config["Cloudinary:ApiKey"];
        var apiSecret = config["Cloudinary:ApiSecret"];

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file.Length == 0)
            return null;

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream)
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return result.SecureUrl.ToString();
    }
}