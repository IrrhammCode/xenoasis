import socket
import struct
import json
import sys

def send_unity_command(cmd_type, params=None, timeout=30):
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.settimeout(timeout)
    s.connect(('127.0.0.1', 6400))
    banner = s.recv(1024)
    
    payload = {'type': cmd_type, 'params': params or {}}
    cmd = json.dumps(payload).encode('utf-8')
    header = struct.pack('>Q', len(cmd))
    s.sendall(header + cmd)
    
    resp_hdr = s.recv(8)
    if len(resp_hdr) != 8:
        raise Exception("Failed to read 8-byte response header")
    resp_len = struct.unpack('>Q', resp_hdr)[0]
    data = b''
    while len(data) < resp_len:
        chunk = s.recv(min(4096, resp_len - len(data)))
        if not chunk:
            break
        data += chunk
    s.close()
    return json.loads(data.decode('utf-8'))

if __name__ == '__main__':
    code = sys.argv[1] if len(sys.argv) > 1 else 'return "pong";'
    res = send_unity_command('execute_code', {'action': 'execute', 'code': code})
    print(json.dumps(res, indent=2))
