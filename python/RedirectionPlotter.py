import socket
import json
import matplotlib.pyplot as plt
import time
from datetime import datetime
import re

plt.ion() # turn on interactive mode (otherwise the window arises not before "show()"
ax1 = plt.subplot(211)
box = ax1.get_position()
ax1.set_position([box.x0, box.y0, box.width * 0.8, box.height])
ax2 = plt.subplot(212)
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


            ax1.clear()
            ax2.clear()
            ax1.set_xticks(list(range(30, 0, -5)))
            ax1.set_title('Redirection amounts over time')
            ax1.set_ylabel('Redirection amount (degrees)')
            for series, color, label in zip((otrs[-60:], rs[-60:], cs[-60:]), 'rgy', ('over time', 'rotational', 'curvature')):
                ax1.plot(ys[-60:], series, color=color, label=label, linewidth=0.5, linestyle="dashed")
            ax1.fill_between(ys[-60:], hybrid[-60:], color='b', alpha=0.2)
            ax1.plot(ys[-60:], hybrid[-60:], color='b', label='hybrid', linewidth=0.5)

            ax2.set_xlabel('Time from start (seconds)', loc='right')
            ax2.set_ylabel('Redirection amount (degrees)')
            ax2.plot(ys[-60:], tots[-60:], color='m', label='Total redirection', linewidth=1)

            ax1.legend(loc='center left', bbox_to_anchor=(1, 0.5))
            ax2.legend()

            plt.pause(0.05)
plt.ioff()