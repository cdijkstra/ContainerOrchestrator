import subprocess


def getIp():
    p = subprocess.Popen(
        'hostname -I | grep -o "^[0-9.]*"', shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    return p.communicate()[0].decode('ascii').strip()


if __name__ == '__main__':
    print(getIp())
