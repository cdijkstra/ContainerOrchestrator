using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ContainerOrchestrator.Base;
using ContainerOrchestrator.Scheduler.ServiceHandlers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using NUnit.Framework;
using Orcastrate;

namespace ContainerOrchestrator.Scheduler.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        //not the most useful test, but good for mocking examples
        [Test]
        public async Task GetNodeStatusAsyncSuccess()
        {
            var service = new NodeServices("http://gimme-a-cook.ie");

            var clientMock = new Moq.Mock<Orcastrater.OrcastraterClient>();

            var forecast = new List<Node>(){
                    new Node{
                        IpAddress= "1.1.1.1",
                        Name="node1",
                        AllCPU= 300,
                        AllMemory= 2000,
                        FreeCPU=150,
                        FreeMemory=1000 },
                    new Node{
                        IpAddress= "2.2.2.2",
                        Name="node2",
                        AllCPU= 222,
                        AllMemory= 2222,
                        FreeCPU=1111,
                        FreeMemory=1111 }
                };
            var fakeResponse = new NodesResponse();
            foreach (var node in forecast)
            {
                fakeResponse.Nodes.Add(node);
            }
            var fakeCall = TestCalls.AsyncUnaryCall(Task.FromResult(fakeResponse), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
            clientMock.Setup(m => m.GetNodeCapacitiesAsync(Moq.It.IsAny<Empty>(), null, null, Moq.It.IsAny<CancellationToken>())).Returns(fakeCall);

            var result = await service.GetNodeStatusAsync(clientMock.Object);

            Assert.AreEqual(2, result.Count);
        }


        [Test]
        public async Task GetNodeStatusAsyncClientNull()
        {
            var service = new NodeServices("http://gimme-a-cook.ie");

            var clientMock = new Moq.Mock<Orcastrater.OrcastraterClient>();

            var fakeResponse = new NodesResponse();

            var fakeCall = TestCalls.AsyncUnaryCall(Task.FromResult(fakeResponse), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
            clientMock.Setup(m => m.GetNodeCapacitiesAsync(Moq.It.IsAny<Empty>(), null, null, Moq.It.IsAny<CancellationToken>())).Returns(fakeCall);

            // Act
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetNodeStatusAsync(null));
        }

    }
}