namespace BudgetCouple.Application.Common.Interfaces;

/// <summary>
/// Interface for attachment storage operations.
/// </summary>
public interface IAttachmentStorage
{
    /// <summary>
    /// Saves a file to storage and returns the storage path.
    /// </summary>
    Task<string> SaveAsync(Guid lancamentoId, string fileName, Stream fileStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a file stream from storage.
    /// </summary>
    Task<Stream> GetAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the full file path for a storage path.
    /// </summary>
    string GetFullPath(string storagePath);
}
