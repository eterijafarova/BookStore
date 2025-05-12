namespace BookShop.BlobStorage
{
    public interface IBlobService
    {
        /// <summary>
        /// Загружает файл в Blob и возвращает публичный URL.
        /// </summary>
        Task<string> UploadFileAsync(IFormFile file, string? fileName = null);
        
        /// <summary>
        /// При необходимости: удаление файла по URL или имени.
        /// </summary>
        Task DeleteFileAsync(string fileName);
    }
}