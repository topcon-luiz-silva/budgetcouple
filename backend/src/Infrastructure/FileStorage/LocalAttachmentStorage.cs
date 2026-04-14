namespace BudgetCouple.Infrastructure.FileStorage;

using BudgetCouple.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Local file system implementation of IAttachmentStorage.
/// </summary>
public class LocalAttachmentStorage : IAttachmentStorage
{
    private readonly string _basePath;

    public LocalAttachmentStorage(IConfiguration configuration)
    {
        var storageBasePath = configuration["Storage:BasePath"] ?? "./storage";
        _basePath = Path.Combine(storageBasePath, "anexos");

        // Ensure directory exists
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveAsync(Guid lancamentoId, string fileName, Stream fileStream, CancellationToken cancellationToken = default)
    {
        // Create directory for this transaction if it doesn't exist
        var lancamentoDir = Path.Combine(_basePath, lancamentoId.ToString());
        if (!Directory.Exists(lancamentoDir))
            Directory.CreateDirectory(lancamentoDir);

        // Generate unique filename with extension
        var fileExtension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(lancamentoDir, uniqueFileName);

        // Save the file
        using (var fileToCreate = File.Create(fullPath))
        {
            await fileStream.CopyToAsync(fileToCreate, cancellationToken);
        }

        // Return relative storage path
        var relativePath = Path.Combine(lancamentoId.ToString(), uniqueFileName).Replace("\\", "/");
        return relativePath;
    }

    public async Task<Stream> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Arquivo não encontrado: {storagePath}");

        // Return a stream that will be disposed by the caller
        return await Task.FromResult(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        // Try to delete empty directory
        var directory = Path.GetDirectoryName(fullPath);
        if (directory != null && Directory.Exists(directory) && !Directory.EnumerateFiles(directory).Any())
        {
            Directory.Delete(directory);
        }

        await Task.CompletedTask;
    }

    public string GetFullPath(string storagePath)
    {
        return Path.Combine(_basePath, storagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
    }
}
