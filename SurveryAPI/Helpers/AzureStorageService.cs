namespace SurveyAPI.Helpers
{
    using Azure.Storage.Blobs;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public class AzureStorageService : IFileStorageService
    {
        private string connectionString;
        public AzureStorageService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorageConnection");

        }

        /// <summary>
        /// Deletes the file at account storage in the azure. 
        /// </summary>
        /// <param name="fileRoute">the url of the img</param>
        /// <param name="containerName">Container name in the cloud.</param>
        /// <returns></returns>
        public async Task DeleteFile(string fileRoute, string containerName)
        {

            if (string.IsNullOrEmpty(fileRoute))
            {
                return;
            }
            var client = new BlobContainerClient(connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            var fileName = Path.GetFileName(fileRoute);
            var blob = client.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync();


        }
        /// <summary>
        /// Edit the file at account storage in the azure. 
        /// </summary>
        /// <param name="containerName">Container name in the cloud.</param>
        /// <param name="file">img as a file.</param>
        /// <param name="fileRoute">the url of the img</param>
        /// <returns></returns>
        public async Task<string> EditFile(string containerName, IFormFile file, string fileRoute)
        {
            await DeleteFile(fileRoute,containerName);
            return await SaveFile(containerName, file);
        }

        /// <summary>
        /// Saving the file at account storage in the azure. 
        /// </summary>
        /// <param name="containerName">Container name in the cloud</param>
        /// <param name="file">img as a file.</param>
        /// <returns></returns>
        public async Task<string> SaveFile(string containerName, IFormFile file)
        {
            var client = new BlobContainerClient(connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            client.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(fileName);
            await blob.UploadAsync(file.OpenReadStream());
            return blob.Uri.ToString();


        }
    }
}
