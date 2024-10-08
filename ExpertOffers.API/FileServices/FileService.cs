﻿using ExpertOffers.Core.ServicesContract;

namespace ExpertOffers.API.FileServices
{
    /// <summary>
    /// Provides file-related services for the application.
    /// </summary>
    public class FileService : IFileServices
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FileService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        /// <param name="environment">The web host environment.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public FileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, ILogger<FileService> logger)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new file in the "Upload" directory.
        /// </summary>
        /// <param name="file">The file to be created.</param>
        /// <returns>The URL of the created file.</returns>
        public async Task<string> CreateFile(IFormFile file)
        {
            try
            {
                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newPath = Path.Combine(_environment.WebRootPath, "Upload", newFileName);

                if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "Upload")))
                {
                    Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "Upload"));
                }

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream).ConfigureAwait(false);
                }

                var baseUrl = GetBaseUrl() + "Upload/" + newFileName;
                return baseUrl;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error creating file", ex);
            }
        }

        /// <summary>
        /// Deletes a file from the "Upload" directory.
        /// </summary>
        /// <param name="fileName">The URL of the file to be deleted.</param>
        public async Task DeleteFile(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogWarning("No file name provided for deletion.");
                return;
            }

            try
            {
                string filePath = Path.Combine(_environment.WebRootPath, "Upload", fileName);
                _logger.LogInformation("Attempting to delete file at path: " + filePath);

                if (System.IO.File.Exists(filePath))
                {
                    _logger.LogInformation("File exists. Deleting...");
                    System.IO.File.Delete(filePath); // Synchronous deletion
                    _logger.LogInformation("File deleted successfully.");
                }
                else
                {
                    _logger.LogWarning("File not found at path: " + filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: " + fileName);
                throw new InvalidOperationException("Error deleting file", ex);
            }
        }



        /// <summary>
        /// Updates a file in the "Upload" directory.
        /// </summary>
        /// <param name="newFile">The new file to be updated.</param>
        /// <param name="currentFileName">The URL of the current file to be replaced.</param>
        /// <returns>The URL of the updated file.</returns>
        public async Task<string> UpdateFile(IFormFile newFile, string? currentFileName)
        {
            await DeleteFile(currentFileName).ConfigureAwait(false);

            return await CreateFile(newFile).ConfigureAwait(false);
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host.Value}/";
        }
    }
}
