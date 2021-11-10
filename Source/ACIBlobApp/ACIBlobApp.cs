using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace ACIBlobApp
{
    public static class ACIBlobApp
    {
        [FunctionName("ACIBlobApp")]
        public static async Task Run([BlobTrigger("aciimageblob/{name}")]
                                        CloudBlockBlob cloudBlockBlob,
                                        [Blob("aciimageblob/{name}", FileAccess.ReadWrite)]
                                        byte[] blobContents,
                                        [Queue("aciimagequeue")]
                                        IAsyncCollector<CloudQueueMessage>
                                        cloudQueueMessageCollector,
                                        ILogger logger)
        {


            logger.LogInformation(cloudBlockBlob.Name);
            var cloudQueueMessage = new CloudQueueMessage(cloudBlockBlob.Name);
            await cloudQueueMessageCollector.AddAsync(cloudQueueMessage);
            var couldDelete = await cloudBlockBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots,
                                                                        null, null, null);
            logger.LogInformation(couldDelete.ToString());

        }
    }
}
