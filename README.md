[![GitHub Pages Deploy Action](https://github.com/benoitgeslain/Visuo-haptic-Illusions-Toolkit/actions/workflows/doxygen-gh-pages.yml/badge.svg)](https://github.com/benoitgeslain/Visuo-haptic-Illusions-Toolkit/actions/workflows/doxygen-gh-pages.yml)
[![pages-build-deployment](https://github.com/benoitgeslain/Visuo-haptic-Illusions-Toolkit/actions/workflows/pages/pages-build-deployment/badge.svg)](https://github.com/benoitgeslain/Visuo-haptic-Illusions-Toolkit/actions/workflows/pages/pages-build-deployment)

# The Visuo-haptic Illusions Toolkit

This toolkit is a an open-source plugin for the Unity game engine. It aims to promote the use of visuo-haptic illusions in the industry (e.g. video games) and in academia by implementing popular visuo-haptic techniques and a simple architecture to create new ones. It also provides low-code tools to debug, log, and visualize the effects of each technique.

## Getting Started

This video (to come) explains the basic concepts of visuo-haptic illusions and this toolkit.

You will find
+ **tutorials** and details about the different modules in the [Wiki](https://github.com/BenoitGeslain/Visuo-haptic-Illusions-Toolkit/wiki),
+ an **overview** of the tookit in the scientific paper (to come),
+ **C# classes** and their documentation in the [autogenerated docs](https://benoitgeslain.github.io/Visuo-haptic-Illusions-Toolkit/),
+ the **source code** on [GitHub](https://github.com/BenoitGeslain/Visuo-haptic-Illusions-Toolkit/).

## List of Implemented Techniques 

### Body Redirection
- [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), Body Warping
- [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), Hybrid Warping
- [Han et al., 2019](http://ieeexplore.ieee.org/document/8260974/), Translational Shift
- [Han et al., 2019](http://ieeexplore.ieee.org/document/8260974/), Interpolated Reach
- [Cheng et al., 2017](http://doi.acm.org/10.1145/3025453.3025753), Sparse Haptics
- [Geslain et al., 2022](https://doi.org/10.1145/3531073.3531100), 2<sup>nd</sup> order polynomials
- [Poupyrev et al., 1996](https://dl.acm.org/doi/10.1145/237091.237102), The Go-Go

### World Redirection
- [Razzaque et al., 2001](http://dx.doi.org/10.2312/egs.20011036), Over Time Rotation
- [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Rotational
- [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Curvature
- [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Redirected Walking Hybrid
- [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), World Warping
- [Williams et al., 2006](https://dl.acm.org/doi/10.1145/1140491.1140495), Translational

### Pseudo-Haptic 
- [Lécuyer et al., 2000](https://doi.org/10.1109/VR.2000.840369), Swamp Illusion
- [Lécuyer et al., 2000](https://doi.org/10.1109/VR.2000.840369), Spring stiffness
- [Samad et al., 2019](https://dl.acm.org/doi/10.1145/3290605.3300550), Pseudo-haptic weight
- [Rietzler et al., 2018](https://dl.acm.org/doi/10.1145/3173574.3173702), Breaking the tracking, weight

### Steering Strategies
- [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Center
- [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Orbit
- [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Targets
- [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Targets + Center

### Not Yet Implemented (Roadmap)

#### World Redirection 
- [Abtahi and Follmer, 2019](https://dl.acm.org/doi/10.1145/3290605.3300752), World-in-miniature

#### Interpolation
- [Kohli et al., 2010](https://doi.org/10.1109/3DUI.2010.5444703), Redirected Touching
- [Zhao et Follmer et al., 2018](https://dl.acm.org/doi/10.1145/3173574.3174118), Complex Boundaries

#### Pseudo-Haptic
- [Gomez Jauregui et al., 2014](http://ieeexplore.ieee.org/document/6777424/), Avatar Weight
- [Argelaguet et al., 2013](https://doi.org/10.1145/2501599), Deformable materials
- [Kasahara et al., 2017](http://doi.acm.org/10.1145/3025453.3025962), Malleable Embodiement

## Reporting Issues

Bug reports and feature requests are very welcome at the [GitHub Issues page](https://github.com/BenoitGeslain/Visuo-haptic-Illusions-Toolkit/issues).

## Contributing

All contributions are welcome. Please find more details on the [wiki](https://github.com/BenoitGeslain/Visuo-haptic-Illusions-Toolkit/wiki/Contributing).

Main contributors:
- Benoît Geslain (benoitgeslain@gmail.com, https://github.com/BenoitGeslain)
- Bruno Jartoux (bruno.jartoux@sii.fr, https://github.com/bjrtx)
- 
Other Contributors:
- Maxence Theriot (https://github.com/Ldeeprenard)
- Théo Machon (https://github.com/TMachon)

## Reference

TODO

## Copyright and Licensing
Copyright (c) 2023-2024 SII Société pour l’Informatique Industrielle


This project is licensed under the open-source MIT X11 license, see the [LICENSE](https://github.com/BenoitGeslain/Visuo-haptic-Illusions-Toolkit/blob/main/LICENSE) file.
In particular, you may not distribute any copy or substantial portion of this project
without the contents of the LICENSE file.
