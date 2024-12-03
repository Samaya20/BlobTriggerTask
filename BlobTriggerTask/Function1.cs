using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("CompressAndConvertImage")]
    [BlobOutput("compressed-container/{name}.jpeg", Connection = "AzureWebJobsStorage")]
    public byte[] Run(
        [BlobTrigger("source-container/{name}", Connection = "AzureWebJobsStorage")] Stream inputBlob,
        string name)
    {
        _logger.LogInformation($"Processing file: {name}");

        using var image = Image.FromStream(inputBlob);
        using var output = new MemoryStream();

        var jpegEncoder = ImageCodecInfo.GetImageEncoders()[1];
        var encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 50L);

        image.Save(output, jpegEncoder, encoderParams);

        _logger.LogInformation("Image compressed and converted successfully.");
        return output.ToArray();
    }
}
