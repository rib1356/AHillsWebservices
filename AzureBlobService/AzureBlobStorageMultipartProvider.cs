using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types

namespace AzureBlobService.Controllers
{
    public class AzureBlobStorageMultipartProvider : MultipartFileStreamProvider
    {
        private CloudBlobContainer _container;
        public AzureBlobStorageMultipartProvider(CloudBlobContainer container)
            : base(Path.GetTempPath())
        {
            _container = container;
            Files = new List<FileDetails>();
        }

        public List<FileDetails> Files { get; set; }

        public override Task ExecutePostProcessingAsync()
        {
            // Upload the files to azure blob storage and remove them from local disk
            foreach (var fileData in this.FileData)
            {
                string fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));

                // Retrieve reference to a blob
                //CloudBlob blob = _container.GetBlobReference(fileName);
                //blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;
                //blob.UploadFile(fileData.LocalFileName);

                CloudBlockBlob blockBlob = _container.GetBlockBlobReference(fileName);
                blockBlob.Properties.ContentType = fileData.Headers.ContentType.MediaType;
                //blockBlob.UploadFromStream()

                using (var fileStream = System.IO.File.OpenRead(@"C:\Users\rib13\Desktop\ninjacat.png"))
                {
                    blockBlob.UploadFromStream(fileStream);
                }

                File.Delete(fileData.LocalFileName);
                Files.Add(new FileDetails
                {
                    ContentType = blockBlob.Properties.ContentType,
                    Name = blockBlob.Name,
                    Size = blockBlob.Properties.Length,
                    Location = blockBlob.Uri.AbsoluteUri
                });
            }

            return base.ExecutePostProcessingAsync();
        }
    }
}