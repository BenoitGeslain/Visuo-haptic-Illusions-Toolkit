"""Script for user redirection """

import builtins
import socket
from gettext import translation
import json
import pathlib
import re
import matplotlib.pyplot as plt


import matplotlib
matplotlib.use('TkAgg')



localedir = pathlib.Path(__file__).parent / 'locales'
translation('messages', localedir,languages=['fr_FR'], fallback=True).install()
_ = builtins.__dict__['_'] #  convince linters '_' is defined


plt.ion() # turn on interactive mode
fig = plt.figure(figsize=(10,6), constrained_layout=True)
ax1 = plt.subplot(211)
box = ax1.get_position()
ax1.set_position([box.x0, box.y0, box.width, box.height * 0.8])
ax2 = plt.subplot(212)
box = ax2.get_position()
ax2.set_position([box.x0, box.y0, box.width, box.height * 0.9])


otrs, rs, cs, otrsSum, rsSum, csSum, ys, maxSums = ([] for _ in range(8))


serversocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# bind the socket to a public host, and a well-known port
serversocket.bind(("localhost", 13000))
# become a server socket
serversocket.listen(5)

while True:
    print("Waiting for socket")
    clientsocket, __ = serversocket.accept()
    with clientsocket:
        while True:
            try:
                chunk = clientsocket.recv(8192).decode()
            except ConnectionResetError:
                print("Connection reset")
                break
            for m in re.finditer(r'\{[^}]*}', chunk):
                print(m[0])
                d = json.loads(m[0])
                for k, l in {
                    "overTime": otrs, "rotational": rs, "curvature": cs, "overTimeSum": otrsSum,
                    "rotationalSum": rsSum, "curvatureSum": csSum, "time": ys, "maxSums": maxSums
                }.items():
                    l.append(d[k])

            ax1.clear()
            ax2.clear()

            ax1.set_ylim((0, 180))
            ax1.set_title(_('Redirection amounts over time'), y=1.35)
            ax1.set_ylabel(_('Redirection Applied to User (degrees)'))
            labels=(_('Over time rotation'), _('Rotational'), _('Curvature'))
            ax1.stackplot(
                ys, otrsSum, rsSum, csSum, *zip(*maxSums),
                labels=labels + labels,
                colors="rgbrgb")
            # ax1.plot(ys[-60:], hybrid[-60:], color='b', label='Hybrid', linewidth=0.5)

            # ax2.set_xticks(list(map(int, ys[::10])))
            # ax2.set_xlim((ys[-1] - 30, ys[-1]))
            ax2.set_xlabel(_('Time from start (seconds)'), loc='right')
            ax2.set_ylabel(_('Redirection per Second (degrees/s)'))
            for series, color, label in zip([otrs, rs, cs], "rgb", labels):
                ax2.plot(ys, series, color=color, label=label, linewidth=1)


            ax1.legend(
                loc='lower left',
                bbox_to_anchor=(0, 1, 1, 0),
                mode="expand",
                ncols=3,
                title=_("Redirection source"))
            plt.pause(0.05)
