using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WhatSAP.Controllers
{
    public static class BlobsController
    {
        public static CloudBlobContainer GetClouldBlobContainer()
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();
            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(Configuration["ConnectionStrings:AzureStorageConnectionString-1"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("whatsap");

            return container;
        }
    }
}