using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ContainerOrchestrator.Api;
using ContainerOrchestrator.Base;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
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
        public async Task ClientBaseServerStreamingCallCanBeMocked()
        {
            // Arrange
            var service = new OrcastrateService(NullLogger<OrcastrateService>.Instance);
            var writer = new Mock<IServerStreamWriter<PodsResponse>>();

            var context = new Mock<ServerCallContext>();
            context.Protected().Setup<string>("PeerCore").Returns("something");
            
            // Act
            await service.Reconcile(new Mock<IAsyncStreamReader<RegisterRequest>>().Object, writer.Object, context.Object);

            // Asser
            writer.Verify(x => x.WriteAsync(It.IsAny<PodsResponse>()));
        }
    }
}