﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if false
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
//using Azure.ResourceManager.Storage.Models;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Network.Tests.Helpers;
using NUnit.Framework;
//using Sku = Azure.ResourceManager.Storage.Models.Sku;
using SubResource = Azure.ResourceManager.Network.Models.SubResource;

namespace Azure.ResourceManager.Network.Tests
{
    public class TroubleshootTests : NetworkServiceClientTestBase
    {
        public TroubleshootTests(bool isAsync) : base(isAsync)
        {
        }

        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                Initialize();
            }
        }

        [Test]
        [Ignore("Track2: The NetworkWathcer is involved, so disable the test")]
        public async Task TroubleshootApiTest()
        {
            string resourceGroupName = Recording.GenerateAssetName("azsmnet");

            string location = "westus2";
            var resourceGroup = CreateResourceGroup(resourceGroupName, location);

            // CreateVirtualNetworkGateway API
            // Prerequisite:- Create PublicIPAddress(Gateway Ip) using Put PublicIPAddress API
            string publicIpName = Recording.GenerateAssetName("azsmnet");
            string domainNameLabel = Recording.GenerateAssetName("azsmnet");

            PublicIPAddress nic1publicIp = await CreateDefaultPublicIpAddress(publicIpName, resourceGroupName, domainNameLabel, location);

            //Prerequisite:-Create Virtual Network using Put VirtualNetwork API
            string vnetName = Recording.GenerateAssetName("azsmnet");
            string subnetName = "GatewaySubnet";

            await CreateVirtualNetwork(vnetName, subnetName, resourceGroupName, location);

            Response<Subnet> getSubnetResponse = await GetVirtualNetworkContainer(resourceGroupName).Get(vnetName).Value.GetSubnets().GetAsync(subnetName);

            // CreateVirtualNetworkGateway API
            string virtualNetworkGatewayName = Recording.GenerateAssetName("azsmnet");
            string ipConfigName = Recording.GenerateAssetName("azsmnet");

            var virtualNetworkGateway = new VirtualNetworkGatewayData()
            {
                Location = location,
                Tags = { { "key", "value" } },
                EnableBgp = false,
                GatewayDefaultSite = null,
                GatewayType = VirtualNetworkGatewayType.Vpn,
                VpnType = VpnType.RouteBased,
                IpConfigurations =
                {
                    new VirtualNetworkGatewayIPConfiguration()
                    {
                        Name = ipConfigName,
                        PrivateIPAllocationMethod = IPAllocationMethod.Dynamic,
                        PublicIPAddress = new SubResource() { Id = nic1publicIp.Id }, Subnet = new SubResource() { Id = getSubnetResponse.Value.Id }
                    }
                },
                Sku = new VirtualNetworkGatewaySku() { Name = VirtualNetworkGatewaySkuName.Basic, Tier = VirtualNetworkGatewaySkuTier.Basic }
            };

            var virtualNetworkGatewayContainer = GetVirtualNetworkGatewayContainer(resourceGroupName);
            var putVirtualNetworkGatewayResponseOperation =
                await virtualNetworkGatewayContainer.CreateOrUpdateAsync(virtualNetworkGatewayName, virtualNetworkGateway);
            await putVirtualNetworkGatewayResponseOperation.WaitForCompletionAsync();;
            // GetVirtualNetworkGateway API
            Response<VirtualNetworkGateway> getVirtualNetworkGatewayResponse =
                await virtualNetworkGatewayContainer.GetAsync(virtualNetworkGatewayName);

            //TODO:There is no need to perform a separate create NetworkWatchers operation
            //Create network Watcher
            //string networkWatcherName = Recording.GenerateAssetName("azsmnet");
            //NetworkWatcher properties = new NetworkWatcher { Location = location };
            //await networkWatcherContainer.CreateOrUpdateAsync(resourceGroupName, networkWatcherName, properties);

            //Create storage
            //string storageName = Recording.GenerateAssetName("azsmnet");
            //var storageParameters = new StorageAccountCreateParameters(new Sku(SkuName.StandardLRS), Kind.Storage, location);

            //Operation<StorageAccount> accountOperation = await StorageManagementClient.StorageAccounts.CreateAsync(resourceGroupName, storageName, storageParameters);
            //Response<StorageAccount> account = await accountOperation.WaitForCompletionAsync();;
            //TroubleshootingParameters parameters = new TroubleshootingParameters(getVirtualNetworkGatewayResponse.Value.Id, account.Value.Id, "https://nwtestdbdzq4xsvskrei6.blob.core.windows.net/vhds");

            ////Get troubleshooting
            //var networkWatcherContainer = GetNetworkWatcherContainer("NetworkWatcherRG");
            //var troubleshootOperation = await networkWatcherContainer.Get("NetworkWatcher_westus2").Value.GetTroubleshootingAsync(parameters);
            //await troubleshootOperation.WaitForCompletionAsync();;

            ////Query last troubleshoot
            //var queryTroubleshootOperation = await networkWatcherContainer.Get("NetworkWatcher_westus2").Value.GetTroubleshootingResultAsync(new QueryTroubleshootingParameters(getVirtualNetworkGatewayResponse.Value.Id));
            //await queryTroubleshootOperation.WaitForCompletionAsync();;
            //TODO: make verification once fixed for troubleshoot API deployed
        }
    }
}
#endif
