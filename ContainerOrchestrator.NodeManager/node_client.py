from __future__ import print_function
import logging

import grpc

import node_info_pb2
import node_info_pb2_grpc
import get_node_ip


def run():
    with grpc.insecure_channel('localhost:50052') as channel:
        stub = node_info_pb2_grpc.NodeInfoStub(channel)
        ip = get_node_ip.getIp()
        response = stub.NodeJoin(node_info_pb2.JoinRequest(nodeIp=ip))
    print(response.isJoin)


if __name__ == '__main__':
    logging.basicConfig()
    run()
