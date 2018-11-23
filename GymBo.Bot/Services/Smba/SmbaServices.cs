using System;
using System.IO;
using System.Threading.Tasks;

using GymBo.Bot.Models;

using Microsoft.Bot.Schema;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json;
using RestSharp;

namespace GymBo.Bot.Services.Smba
{
    public class SmbaServices : ISmbaServices
    {
        public SmbaServices(string appId, string appSecret, string blobConnection)
        {
            AppId = appId;
            AppSecret = appSecret;
            BlobConnection = blobConnection;
        }

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string BlobConnection { get; set; }

        public async Task<Uri> GetAttachmentAsync(Attachment attachment)
        {
            try
            {
                var token = GetToken();
                var client = new RestClient(attachment.ContentUrl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("authorization", $"Bearer {token}");
                IRestResponse response = client.Execute(request);

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(BlobConnection);

                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("appcontainer");

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(attachment.Name);

                MemoryStream stream = new MemoryStream(response.RawBytes);

                await cloudBlockBlob.UploadFromStreamAsync(stream);

                return cloudBlockBlob.Uri;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string GetToken()
        {
            try
            {
                if (!string.IsNullOrEmpty(AppId) && !string.IsNullOrEmpty(AppSecret))
                {
                    var client = new RestClient("https://login.microsoftonline.com/botframework.com/oauth2/v2.0/token");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("application/x-www-form-urlencoded",
                        $"grant_type=client_credentials&client_id={AppId}&client_secret={AppSecret}&scope=https%3A%2F%2Fapi.botframework.com%2F.default",
                        ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    var myToken = JsonConvert.DeserializeObject<MyToken>(response.Content);
                    return myToken.AccessToken;
                }

                throw new ArgumentException();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
