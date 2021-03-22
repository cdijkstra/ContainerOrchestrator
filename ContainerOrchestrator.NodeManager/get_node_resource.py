import psutil

# https://kubernetes.io/docs/concepts/configuration/manage-resources-containers/#resource-units-in-kubernetes

# 1 vCPU/Core for cloud providers and 1 hyperthread
cpu_core = psutil.cpu_count()
# in Mib
memory_total = psutil.virtual_memory()[0]/2**20
