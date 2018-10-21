using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TryIt
{
    class BlobProgram
    {
        
        public static void BlobMain(string[] args)
        {
            string url = "https://mksdevblob.blob.core.windows.net/mywebiislog/MKSDEVWEBAPP/2018/09/23/13/a544f2.log";
            ProcessMessagesAsync(url).GetAwaiter().GetResult();
        }
        private static string connStr = "DefaultEndpointsProtocol=https;AccountName=mksdevblob;AccountKey=bx2fR38ypjvDlhySPDbxRLClnRwebm46b4pQ5Oyy2g9sxcdx9GP+Mbf9RWLqEJ0ffbAjiC7EVu9VwvOa+laKLA==;EndpointSuffix=core.windows.net";
        private static readonly CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);
        private static readonly CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        private static string cacheConnStr = "DefaultEndpointsProtocol=https;AccountName=mksdevblobcache;AccountKey=lK1VssSouxoZVLJd0K8conyI+60+EoYl5sJSgSAOOKC/Fjf5l24t/bCcTE/PSuedwXoCnX4vi0Ffvb5TL7XVtg==;EndpointSuffix=core.windows.net";
        private static readonly CloudStorageAccount cacheStorageAccount = CloudStorageAccount.Parse(cacheConnStr);
        private static readonly CloudBlobClient cacheBlobClient = cacheStorageAccount.CreateCloudBlobClient();
        private static readonly CloudBlobContainer cacheContainer = cacheBlobClient.GetContainerReference("mkswebiislogcursor");

        public async static Task ProcessMessagesAsync(
            string blobUris)
        {
            bool stopRedundantTransfer = false;
            bool blobCursorupdateStatus = false;
            long cursorPosition=0;
            if (Uri.TryCreate(blobUris, UriKind.Absolute, out Uri validUri))
            {
                //TODO send CancellationToken
                var blobItem = await blobClient.GetBlobReferenceFromServerAsync(validUri);

                if (blobItem.GetType() == typeof(CloudAppendBlob))
                {
                    CloudAppendBlob appBlob = (CloudAppendBlob)blobItem;
                    //TODO send CancellationToken
                    string blobData = await appBlob.DownloadTextAsync();
                    Console.WriteLine(blobData);
                }
                else if(blobItem.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob appBlob = (CloudBlockBlob)blobItem;
                    
                    if(!long.TryParse(appBlob.Properties.Length.ToString(), out long newLength))
                    {
                        newLength = 0;
                    }

                    while(!blobCursorupdateStatus)
                    {
                        CloudBlockBlob blob = cacheContainer.GetBlockBlobReference(appBlob.Name);
                        var position = await GetBlobReadPosition(blob);
                        //2669 < 3050
                        if(position < newLength)
                        {
                            blobCursorupdateStatus = await UpdateBlobReadCursorPosition(blob, newLength.ToString());
                            cursorPosition = position;
                        }
                        else if(position >= newLength)
                        {
                            blobCursorupdateStatus = true;
                            stopRedundantTransfer = true;
                        }
                    }

                    if (!stopRedundantTransfer)
                    {
                        string value = null;
                        using (var stream = new MemoryStream())
                        {
                            await appBlob.DownloadToStreamAsync(stream);
                            //Console.WriteLine($"Length: {stream.Length}");

                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                stream.Position = cursorPosition;
                                while (!reader.EndOfStream)
                                {
                                    value = await reader.ReadLineAsync();
                                    Console.WriteLine(value);
                                }
                            }
                        }
                    }
                }
            }
            Console.ReadLine();
        }

        private async static Task<long> GetBlobReadPosition(CloudBlockBlob blob)
        {
            //long length = blob.Properties.Length;
            var blobExists = await blob.ExistsAsync();
            long currPosition = 0;
            if (blobExists)
            {
                var blobValue = await blob.DownloadTextAsync();
                long.TryParse(blobValue, out currPosition);
            }
            else
            {
                await blob.UploadTextAsync("0");
            }
            return currPosition;
        }

        private async static Task<bool> UpdateBlobReadCursorPosition(CloudBlockBlob blob, string length)
        {
            try
            {
                await blob.UploadTextAsync(length, accessCondition:
                AccessCondition.GenerateIfMatchCondition(blob.Properties.ETag), options:null, operationContext:null);
                return true;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
                {
                    Console.WriteLine("Precondition failure as expected. Blob's orignal etag no longer matches");
                    return false;
                    // TODO: client can decide on how it wants to handle the 3rd party updated content.
                }
                else
                    throw;
            }
        }
    }

}