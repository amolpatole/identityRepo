using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApi.Helpers
{
    public class StorageAccountHelper
    {
        private string storageConnectionString;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;

        public string StorageConnectionString
        {
            get { return storageConnectionString; }
            set
            {
                this.storageConnectionString = value;
                // Storage account object creation static method.
                storageAccount = CloudStorageAccount.Parse(this.storageConnectionString);
            }
        }

        public async Task SendMessageToQueueAsync(string message, string queueName)
        {
            queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            CloudQueueMessage queueMessage = new CloudQueueMessage(message);
            await queue.AddMessageAsync(queueMessage);
        }
    }
}
