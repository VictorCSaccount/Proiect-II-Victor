namespace ProiectII.Interfaces
{
    public interface IFileStorageService
    {

        // avem nevoie de 2 metode - una pentru a salva imaginea pe disc si alta pentru a sterge imaginea de pe disc

        Task<string> SaveFileAsync(IFormFile file, string folderName);
        void DeleteFile(string filePath);



    }
}
