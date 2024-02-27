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
            ax1.set_title('Redirection amounts over time')
            ax1.set_ylabel('Redirection per second (degrees)')
            # for series, color, label in zip((otrs, rs, cs), 'rgy', ('Over time\nrotation', 'Rotational', 'Curvature')):
            #     ax1.plot(ys[-60:], series[-60:], color=color, label=label, linewidth=0.5, linestyle="dashed")
            ax1.stackplot(ys[-60:], otrs[-60:], rs[-60:], cs[-60:], labels=('Over time\nrotation', 'Rotational', 'Curvature'), linewidth=0.5, linestyle="dashed")
            # ax1.fill_between(ys[-60:], hybrid[-60:], color='b', alpha=0.2)
            # ax1.plot(ys[-60:], hybrid[-60:], color='b', label='Hybrid', linewidth=0.5)

            ax2.set_xticks(list(map(int, ys[::10])))
            ax2.set_xlim((ys[-1] - 30, ys[-1]))
            ax2.set_xlabel('Time from start (seconds)', loc='right')
            ax2.set_ylabel('Redirection amount (degrees)')
            ax2.plot(ys[-60:], tots[-60:], color='m', label='Total\nredirection', linewidth=1)

            for ax in (ax1, ax2):
                ax.legend(loc='center left', bbox_to_anchor=(1, 0.5))
            plt.pause(0.05)