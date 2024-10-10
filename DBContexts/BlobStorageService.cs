//using MoviePlaylist.Models;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MoviePlaylist.DBContexts;
//using Azure.Storage.Blobs;
//using Models;

//namespace MoviePlaylist.DBContexts
//{
//    public class BlobStorageService : IBlobStorageService
//    {
//        private readonly BlobContainerClient _containerClient;

//        // Constructor to initialize Blob Storage container client
//        public BlobStorageService(string connectionString, string containerName)
//        {
//            var blobServiceClient = new BlobServiceClient(connectionString);
//            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
//            _containerClient.CreateIfNotExists();
//        }

//        // Method to archive a completed playlist to Blob Storage
//        public async Task ArchivePlaylistAsync(UserCurrentPlaylist playlist)
//        {
//            var blobName = $"{playlist.PlaylistId}.json";
//            var blobClient = _containerClient.GetBlobClient(blobName);

//            using (var stream = new MemoryStream())
//            {
//                // Serialize the playlist to JSON and upload it to the blob
//                var json = JsonConvert.SerializeObject(playlist);
//                var writer = new StreamWriter(stream);
//                writer.Write(json);
//                writer.Flush();
//                stream.Position = 0;

//                // Upload the playlist JSON to Blob Storage
//                await blobClient.UploadAsync(stream, overwrite: true);
//            }
//        }
//    }
//}
