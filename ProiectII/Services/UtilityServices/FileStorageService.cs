
// realizat cu ajutorul gemini!!!! Aici e porblema mare cu fisirerele di se asta am ales varianta realizata de chat...

//ideea e ca atacatorul trimite fisiere de tip exe fisere prea mari sau ne impiedica scalare applicatie.... 
// la problema cu salvarea fisiereor d etip imagine, trebuie sa ne asiguram ca nu permitem fisiere de tip executabil sau alte tipuri periculoase, si sa limitam dimensiunea fisierelor pentru a preveni atacurile de tip DoS (Denial of Service) prin upload de fisiere foarte mari. De asemenea, este important sa salvam fisierele intr-un folder dedicat si sa generam nume unice pentru a evita coliziunile si pentru a preveni accesul neautorizat la alte fisiere.
// plus dimensouena fisirerlor trebuie sa fie limitata....(nu vrem ca un user bine intentionat sa ne trimita un fisier de 1TB care sa blcheze stocare


using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using ProiectII.Interfaces;

namespace ProiectII.Services.UtilityServices
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly string[] _allowedExtensions;
        private readonly long _maxFileSizeInBytes;

        public FileStorageService(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;

            // Citim din configurație, nu mai hardcodăm
            _allowedExtensions = _config.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
                                 ?? new[] { ".jpg", ".png" };

            int maxMb = _config.GetValue<int>("FileStorage:MaxFileSizeMB", 2); // Default 2MB
            _maxFileSizeInBytes = maxMb * 1024 * 1024;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Eroare: Fișierul este nul sau gol.");

            if (file.Length > _maxFileSizeInBytes)
                throw new InvalidOperationException($"Eroare: Fișierul depășește limita de {_config.GetValue<int>("FileStorage:MaxFileSizeMB")} MB.");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Eroare: Tipul de fișier nu este permis.");

            // Sanitizarea parametrului folderName pentru a preveni Path Traversal
            string safeFolderName = Path.GetFileName(folderName);

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", safeFolderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Returnăm calea mereu cu forward slash pentru compatibilitate Web
            return $"/uploads/{safeFolderName}/{fileName}";
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            // Evităm erorile pe Linux/Docker prin normalizarea separatorului
            string normalizedPath = filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string fullPath = Path.Combine(_env.WebRootPath, normalizedPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
