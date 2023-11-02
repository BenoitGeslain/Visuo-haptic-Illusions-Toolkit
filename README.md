# The Visuo-haptic Illusions Toolkit

This toolkit for visuo-haptic illusions is a set of open-source tools developped for the Unity game engine under th MIT licence. It aims to promote the use of visuo-haptic illusions in the industry (e.g. video game) and in research by implementing popular visuo-haptic techniques, a simple architecture to create new ones. It also provides tools to help in debugging, logging and visualizing the effects of each technqiue requiring no or minimal additionnal programming.
Unity plugin for using and developing visuo-haptic illusions in virtual reality.

## What is the Visuo-haptic Illusions Toolkit

This video explains the basic concepts of visuo-haptic illusions and this toolkit.


More details can be found inside the [paper published in XXX]() and in the [wiki]().

## Getting Started

To get started, you can check the [wiki](), it contains tutorials,  details about the different modules in this toolkit as well as a [doxygen documentation]().

## List of Techniques Implemented

The following tables list all the techniques implemented

| Body Redirection  |    |
|:-------------- | :--------------: |
| [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), Body Warping    |[Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), Hybrid Warping    |
| [Han et al., 2019](http://ieeexplore.ieee.org/document/8260974/), Translational Shift    | [Han et al., 2019](http://ieeexplore.ieee.org/document/8260974/), Interpolated Reach *    |
| [Cheng et al., 2017](http://doi.acm.org/10.1145/3025453.3025753), Sparse Haptics   | [Geslain et al., 2022](https://doi.org/10.1145/3531073.3531100), 2<sup>nd</sup> order polynomials   |
| [Poupyrev et al., 1996](https://dl.acm.org/doi/10.1145/237091.237102), The Go-Go   |
| World Redirection   |     |
| [Razzaque et al., 2001](http://dx.doi.org/10.2312/egs.20011036), Over Time Rotation    | [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Rotational <br />[Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/)    |
| [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Curvature <br />[Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/)    | [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Redirected Walking Hybrid    |
| [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), World Warping    | [Williams et al., 2006](https://dl.acm.org/doi/10.1145/1140491.1140495), Translational <br />[Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/),    |
| Interpolation   |     |
| Pseudo-Haptic   |     |
| [Lécuyer et al., 2000](https://doi.org/10.1109/VR.2000.840369), Swamp Illusion   | [Lécuyer et al., 2000](https://doi.org/10.1109/VR.2000.840369), Spring stiffness   |
| [Samad et al., 2019](https://dl.acm.org/doi/10.1145/3290605.3300550), Pseudo-haptic weight   | [Rietzler et al., 2018](https://dl.acm.org/doi/10.1145/3173574.3173702), Breaking the tracking, weight   |

\* Erreur de signe dans l'équation à 3.3, la technique redirige dans la direction opposée. + B est faux

| Steering Strategies  | Status   |
|:-------------- | :--------------: |
| [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Center    | [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Orbit    |   [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Targets    | &check; |
| [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Targets + Center    |	|

### Not Yet Implemented

| World Redirection   |     |
|:-------------- | :--------------: |
| [Abtahi and Follmer, 2019](https://dl.acm.org/doi/10.1145/3290605.3300752), World-in-miniature   | &cross;   |
| Interpolation   |     |
| [Kohli et al., 2010](https://doi.org/10.1109/3DUI.2010.5444703), Redirected Touching<br />[Kohli, 2013](https://doi.org/10.17615/34cy-pt44)   | &cross;   |
| [Zhao et Follmer et al., 2018](https://dl.acm.org/doi/10.1145/3173574.3174118), Complex Boundaries   | &cross;   |
| Pseudo-Haptic   |     |
| [Gomez Jauregui et al., 2014](http://ieeexplore.ieee.org/document/6777424/), Avatar Weight   | &cross;   |
| [Argelaguet et al., 2013](https://doi.org/10.1145/2501599), Deformable materials   | &cross;   |
| [Kasahara et al., 2017](http://doi.acm.org/10.1145/3025453.3025962), Malleable Embodiement   | &cross;   |

## Authors
Benoît Geslain (benoit.geslain@sii.fr, https://github.com/BenoitGeslain), Bruno Jartoux (bruno.jartoux@sii.fr, https://github.com/bjrtx)

## Copyright and licensing
Copyright (c) 2023 SII Société pour l’Informatique Industrielle


This project is licensed under the open-source MIT X11 license, see the LICENSE file.
In particular, you may not distribute any copy or substantial portion of this project
without the contents of the LICENSE file.


## TODO

- Readme
	- ajouter bandeau
	- reduire la taille du tableau
- Wiki
- Logging
	- jouer une scène à partir des logs
- Simulation
	- Générer trajectoire de main avec
		- le Minimum Jerk Model
		- une courbe de Bézier ?
	- Générer trajectoire de marche avec ???
- Redirection
	- Tester chaque fonction de redirection
- Pseudo-haptique
	- Ajouter les paramètres à ParametersToolkit
