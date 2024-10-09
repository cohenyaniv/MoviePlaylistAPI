using Azure.Storage.Queues;
using System.Threading.Tasks;

namespace MoviePlaylist.Services
{
    public class QueueService
    {
        private readonly QueueClient _queueClient;

        // Constructor to initialize Azure Queue Storage client
        public QueueService(string connectionString, string queueName)
        {
            _queueClient = new QueueClient(connectionString, queueName);
            _queueClient.CreateIfNotExists();
        }

        // Method to queue a playlist for archiving
        public async Task QueuePlaylistForArchivingAsync(string playlistId)
        {
            // Send the playlist ID as a message in the queue
            await _queueClient.SendMessageAsync(playlistId);
        }

        // Method to receive a message from the queue (playlist ID)
        public async Task<string> ReceiveMessageAsync()
        {
            var response = await _queueClient.ReceiveMessageAsync();
            if (response.Value != null)
            {
                var message = response.Value.MessageText;
                // Delete the message from the queue after receiving it
                await _queueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);
                return message;
            }
            return null;
        }
    }
}
