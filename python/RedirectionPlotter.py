import socket
import json
import matplotlib.pyplot as plt
import time
from datetime import datetime
import re

plt.ion() # turn on interactive mode (otherwise the window arises not before "show()"
fig = plt.figure()
ax = fig.gca()
otrs, rs, cs, hybrid, tots, ys = ([] for _ in range(6))




serversocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# bind the socket to a public host, and a well-known port
serversocket.bind(("localhost", 13000))
# become a server socket
serversocket.listen(1)

while True:
    # accept connections from outside
    clientsocket, _ = serversocket.accept()
    while clientsocket:
        chunk = clientsocket.recv(4096).decode()
        print(chunk)
        for m in re.finditer(r'\{[^}]*}', chunk):
            d = json.loads(m[0])
            otrs.append(d["overTime"])
            rs.append(d["rotational"])
            cs.append(d["curvature"])
            hybrid.append(d["hybrid"])
            tots.append(d["total"])
            ys.append(d["time"])
            ax.clear()
            ax.set_title('Redirection amounts over time')
            ax.set_ylabel('Redirection amount (degrees)')
            ax.set_xlabel('Time from start (seconds)', loc='right')
            for series, color, label in zip((otrs, rs, cs, hybrid), 'rgbym', ('over time', 'rotational', 'curvature', 'hybrid', 'total')):
                ax.plot(ys, series, color=color, label=label)
            ax.legend()

            plt.pause(0.05)
plt.ioff()