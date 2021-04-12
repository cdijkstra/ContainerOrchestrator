using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ContainerOrchestrator.Base;
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
        public void ClientBaseServerStreamingCallCanBeMocked()
        {
            // var mockClient = new Moq.Mock<Orcastrater.OrcastraterClient>();
            // var forecast = new List<Node>(){
            //         new Node{
            //             IpAddress= "1.1.1.1",
            //             Name="node1",
            //             AllCPU= 300,
            //             AllMemory= 2000,
            //             FreeCPU=150,
            //             FreeMemory=1000 },
            //         new Node{
            //             IpAddress= "2.2.2.2",
            //             Name="node2",
            //             AllCPU= 222,
            //             AllMemory= 2222,
            //             FreeCPU=1111,
            //             FreeMemory=1111 }
            //     };
            // var jsonString = JsonSerializer.Serialize(forecast);
            // var ret = new Orcastrate.GenericMessage { Content = jsonString };
            //
            // var mockResponseStream = new Moq.Mock<IAsyncStreamReader<GenericMessage>>();
            // mockResponseStream.Setup(sr=> sr.Current).Returns(ret);
            // var fakeCall = TestCalls.AsyncServerStreamingCall<GenericMessage>(mockResponseStream.Object, Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
            // mockClient.Setup(m => m.GetNodeCapacities(Moq.It.IsAny<Empty>(), null, null, CancellationToken.None)).Returns(fakeCall);
            // Assert.AreSame(fakeCall, mockClient.Object.GetNodeCapacities(new Empty()));

        }
    }
}