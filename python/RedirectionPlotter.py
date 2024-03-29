"""Script for user redirection """

import builtins
import socket
from gettext import translation
import json
import pathlib
import matplotlib
import matplotlib.pyplot as plt


matplotlib.use('TkAgg')



localedir = pathlib.Path(__file__).parent / 'locales'
translation('messages', localedir,languages=['fr_FR'], fallback=True).install()
_ = builtins.__dict__['_']  # Convince linters '_' is defined


plt.ion()  # Turn on interactive mode
fig = plt.figure(figsize=(10,6), constrained_layout=True)
ax1 = plt.subplot(211)
box = ax1.get_position()
ax1.set_position((box.x0, box.y0, box.width, box.height * 0.8))
ax2 = plt.subplot(212)
box = ax2.get_position()
ax2.set_position((box.x0, box.y0, box.width, box.height * 0.9))


otrs, rs, cs, otrsSum, rsSum, csSum, ys, maxSums = ([] for _ in range(8))


with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as serversocket:
    serversocket.bind(("localhost", 13000))
    serversocket.listen(5)
    with serversocket.accept()[0] as clientsocket:
        with clientsocket.makefile() as file:
            for line in file:
                print(line)
                if not plt.get_fignums():
                    break
                d = json.loads(line)
                for k, l in {
                    "overTime": otrs, "rotational": rs, "curvature": cs, "overTimeSum": otrsSum,
                    "rotationalSum": rsSum, "curvatureSum": csSum, "time": ys, "maxSums": maxSums
                }.items():
                    l.append(d[k])

                ax1.clear()
                ax2.clear()

                ax1.set_ylim((0, 180))
                ax1.set_title(_('Redirection amounts over time'), y=1.35)
                ax1.set_ylabel(_('Redirection Applied to\nUser (degrees)'))
                labels=(_('Over time rotation'), _('Rotational'), _('Curvature'))
                ax1.stackplot(
                    ys, otrsSum, rsSum, csSum,
                    labels=list(labels),
                    # ys, *zip(*maxSums),
                    # labels=list(labels) + [l + '_max' for l in labels],
                    colors="rgb")

                # ax2.set_ylim((-0.1, 7.5))
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
