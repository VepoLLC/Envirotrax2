using System.Security.Cryptography;
using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.Common.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Helpers;

public class PdfViewService : IPdfViewService
{
    private const int MaxFileNameLength = 100;
    private const string DefaultFileName = "document.pdf";
    private const string PdfExtension = ".pdf";

    private static readonly TimeSpan _lifetime = TimeSpan.FromMinutes(10);
    private static readonly byte[] _pdfSignature = "%PDF-"u8.ToArray();

    private readonly IMemoryCache _cache;

    public PdfViewService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public PdfViewDto Store(byte[] content, string fileName)
    {
        if (!content.AsSpan().StartsWith(_pdfSignature))
        {
            throw new AppValidationException("The uploaded file is not a PDF document.");
        }

        var view = new PdfViewDto
        {
            Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
            FileName = SanitizeFileName(fileName),
            Content = content
        };

        _cache.Set(view.Token, view, _lifetime);

        return view;
    }

    public PdfViewDto? Find(string token)
    {
        return _cache.TryGetValue(token, out PdfViewDto? view) ? view : null;
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var name = new string(Path.GetFileName(fileName).Where(c => !invalidCharacters.Contains(c) && c < 128).ToArray()).Trim();

        if (name.Length == 0)
        {
            return DefaultFileName;
        }

        if (!name.EndsWith(PdfExtension, StringComparison.OrdinalIgnoreCase))
        {
            name += PdfExtension;
        }

        return name.Length > MaxFileNameLength
            ? string.Concat(name.AsSpan(0, MaxFileNameLength - PdfExtension.Length), PdfExtension)
            : name;
    }
}
