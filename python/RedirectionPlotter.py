import socket
import json
import matplotlib.pyplot as plt
import re

import matplotlib
matplotlib.use('TkAgg')

plt.ion() # turn on interactive mode
fig = plt.figure(figsize=(10,6), constrained_layout=True)
ax1 = plt.subplot(211)
box = ax1.get_position()
ax1.set_position([box.x0, box.y0, box.width * 0.9, box.height])
ax2 = plt.subplot(212)
box = ax2.get_position()
ax2.set_position([box.x0, box.y0, box.width * 0.9, box.height])


otrs, rs, cs, hybrid, tots, ys = ([] for _ in range(6))


serversocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# bind the socket to a public host, and a well-known port
serversocket.bind(("localhost", 13000))
# become a server socket
serversocket.listen(5)

while True:
    print("Waiting for socket")
    clientsocket, _ = serversocket.accept()
    with clientsocket:
        while True:
            try:
                chunk = clientsocket.recv(4096).decode()
            except ConnectionResetError:
                print("Connection reset")
                break
            for m in re.finditer(r'\{[^}]*}', chunk):
                d = json.loads(m[0])
                for k, l in {"overTime": otrs, "rotational": rs, "curvature": cs, "hybrid": hybrid, "total": tots, "time": ys}.items():
                    l.append(d[k])

            ax1.clear()
            ax2.clear()

            ax1.set_xticks(list(map(int, ys[::10])))
            ax1.set_xlim((ys[-1] - 30, ys[-1]))
            ax1.set_ylim((0, 190))
            ax1.set_title('Redirection amounts over time')
            ax1.set_ylabel('Redirection (degrees)')
            ax1.stackplot(ys[-60:], otrs[-60:], rs[-60:], cs[-60:], labels=('Over time\nrotation', 'Rotational', 'Curvature'), linewidth=0.5, linestyle="dashed")

            # ax2.set_xticks(list(map(int, ys[::10])))
            # ax2.set_xlim((ys[-1] - 30, ys[-1]))
            ax2.set_ylim((0, 190))
            # ax2.set_title('Redirection amounts over time')
            ax2.set_ylabel('Redirection (degrees)')
            ax2.stackplot(ys, otrs, rs, cs, labels=('Over time\nrotation', 'Rotational', 'Curvature'), linewidth=0.5, linestyle="dashed")

            for ax in (ax1, ax2):
                ax.legend(loc='center left', bbox_to_anchor=(1, 0.5))
            plt.pause(0.05)