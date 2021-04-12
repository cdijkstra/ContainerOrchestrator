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

        [Test]
        public async Task MagicMock()
        {
            var service = new NodeServices("http://127.0.0.1");

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


            clientMock.Setup(m => m.GetNodeCapacitiesAsync(Moq.It.IsAny<Empty>(),null,null,Moq.It.IsAny<CancellationToken>())).Returns(fakeCall);


            var result = await service.GetNodeStatusAsync(clientMock.Object);

            Assert.AreEqual(2, result.Count);

        }

        //[Test]
        //public async Task ClientBaseServerStreamingCallCanBeMocked()
        //{
        //    var mockClient = new Moq.Mock<Orcastrater.OrcastraterClient>();
        //    var forecast = new List<Node>(){
        //            new Node{
        //                IpAddress= "1.1.1.1",
        //                Name="node1",
        //                AllCPU= 300,
        //                AllMemory= 2000,
        //                FreeCPU=150,
        //                FreeMemory=1000 },
        //            new Node{
        //                IpAddress= "2.2.2.2",
        //                Name="node2",
        //                AllCPU= 222,
        //                AllMemory= 2222,
        //                FreeCPU=1111,
        //                FreeMemory=1111 }
        //        };
        //    var fakeResponse = new NodesResponse();
        //    foreach (var node in forecast)
        //    {
        //        fakeResponse.Nodes.Add(node);
        //    }
        //    mockClient.Setup(m => m.GetNodeCapacities(Moq.It.IsAny<Empty>(), null, null, CancellationToken.None)).Returns(fakeResponse);

        //    var mockRequestStream = new Moq.Mock<IClientStreamWriter<RegisterRequest>>();

        //    var mockResponseStream = new Moq.Mock<IAsyncStreamReader<PodsResponse>>();
        //    var pods = new PodsResponse();
        //    pods.Pods.Add(new Pod() { Name = "p" + 1 + "-a", Image = "nginx" });
        //    pods.Pods.Add(new Pod() { Name = "p" + 1 + "-b", Image = "nginx" });
        //    pods.Pods.Add(new Pod() { Name = "p" + 1 + "-c-pending-forever", Image = "nginx", Request = new Limitation() { CPU = 10000, Memory = 100 } });
        //    mockResponseStream.Setup(sr => sr.Current).Returns(pods);

        //    // Use a factory method provided by Grpc.Core.Testing.TestCalls to create an instance of a call.
        //    var fakeCall = TestCalls.AsyncDuplexStreamingCall<RegisterRequest, PodsResponse>(mockRequestStream.Object, mockResponseStream.Object, Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
        //    mockClient.Setup(m => m.Reconcile(null, null, CancellationToken.None)).Returns(fakeCall);

        //    //SchedulePodsAsync
        //    PodsRequest result = null;
        //    mockClient.Setup(m => m.SchedulePodsAsync(Moq.It.IsAny<PodsRequest>(), null, null, CancellationToken.None)).Callback<PodsRequest, Metadata, DateTime?, CancellationToken>((r1, r2, r3, r4) => result = r1);
        //    var ps = new PodServices("https://localhost:5001");
        //    await ps.ReconcileAsync();
        //    Assert.AreSame(fakeResponse, result);

        //}
    }
}

//var mockClient = new Moq.Mock<Orcastrater.OrcastraterClient>();
//var forecast = new List<Node>(){
//        new Node{
//            IpAddress= "1.1.1.1",
//            Name="node1",
//            AllCPU= 300,
//            AllMemory= 2000,
//            FreeCPU=150,
//            FreeMemory=1000 },
//        new Node{
//            IpAddress= "2.2.2.2",
//            Name="node2",
//            AllCPU= 222,
//            AllMemory= 2222,
//            FreeCPU=1111,
//            FreeMemory=1111 }
//    };
//var jsonString = JsonSerializer.Serialize(forecast);
//var ret = new Orcastrate.GenericMessage { Content = jsonString };

//var mockResponseStream = new Moq.Mock<IAsyncStreamReader<GenericMessage>>();
//mockResponseStream.Setup(sr=> sr.Current).Returns(ret);
//var fakeCall = TestCalls.AsyncServerStreamingCall<GenericMessage>(mockResponseStream.Object, Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
//mockClient.Setup(m => m.GetNodeCapacities(Moq.It.IsAny<Empty>(), null, null, CancellationToken.None)).Returns(fakeCall);
//Assert.AreSame(fakeCall, mockClient.Object.GetNodeCapacities(new Empty()));