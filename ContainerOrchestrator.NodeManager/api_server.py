from concurrent import futures
import logging

import grpc

import node_info_pb2
import node_info_pb2_grpc


class NodeJoin(node_info_pb2_grpc.NodeInfoServicer):

    def NodeJoin(self, request, context):
        print(request.nodeIp)
        return node_info_pb2.JoinReply(isJoin=('192.168.64.2' == request.nodeIp))


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    node_info_pb2_grpc.add_NodeInfoServicer_to_server(NodeJoin(), server)
    server.add_insecure_port('[::]:50052')
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()
