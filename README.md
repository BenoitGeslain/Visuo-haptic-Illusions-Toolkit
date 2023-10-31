# Visuo-haptic Illusions

This project is a Unity plugin for using and developing visuo-haptic illusions in virtual reality.

- Logging
  - Automatiquement logger tous les champs de scene pour n'avoir qu'à modifier la classe sans aller dans logging.
- Simulation
  - Générer trajectoire de main avec
    - le Minimum Jerk Model
    - une courbe de Bézier ?
  - Générer trajectoire de marche avec ???
- Redirection
  - Cacher les éléments non pertinents en fonction de la méthode sélectionnée
- User Input
  - Ajouter déplacements et rotation de la tête
- Fonctionnalité : Avoir plusieurs ToolkitXXXRedirection in the scene pour combiner World et Body Warping
- Pseudo-haptique
  - Ajouter les paramètres à ParametersToolkit

## Dependencies

- [Unity XR Plugin](https://github.com/ValveSoftware/unity-xr-plugin)
- Text Mesh Pro

## Techniques d'interaction

### Must Have

| Body Redirection  | Status   |
|:-------------- | :--------------: |
| [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), Body Warping    | &check; |
| [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), Hybrid Warping    | &check; |
| [Han et al., 2019](http://ieeexplore.ieee.org/document/8260974/), Continous *    | &check; |
| [Cheng et al., 2017](http://doi.acm.org/10.1145/3025453.3025753), Sparse Haptics   | &check;   |
| [Geslain et al., 2022](https://doi.org/10.1145/3531073.3531100), 2<sup>nd</sup> order polynomials   | &check;   |
| World Redirection   |     |
| [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), World Warping    | &check; |
| [Razzaque et al., 2001](http://dx.doi.org/10.2312/egs.20011036), Over Time Rotation    | &check;   |
| [Williams et al., 2006](https://dl.acm.org/doi/10.1145/1140491.1140495), Translational <br />[Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/),    | &check;   |
| [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Rotational <br />[Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/)    | &check;   |
| [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Curvature <br />[Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/)    | &check;   |
| [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Redirected Walking Hybrid    | &check;   |
| Interpolation   |     |
| [Kohli et al., 2010](https://doi.org/10.1109/3DUI.2010.5444703), Redirected Touching<br />[Kohli, 2013](https://doi.org/10.17615/34cy-pt44)   | &cross;   |
| Pseudo-Haptic   |     |
| [Lécuyer et al., 2000](https://doi.org/10.1109/VR.2000.840369), Spring stiffness   | &cross;   |
| [Rietzler et al., 2018](https://dl.acm.org/doi/10.1145/3173574.3173702), Breaking the tracking, weight   | &cross;   |

\* Erreur de signe dans l'équation à 3.3, la technique redirige dans la direction opposée. + B est faux

| Steering Strategies  | Status   |
|:-------------- | :--------------: |
| [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Center    | &check; |
| [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Orbit    | &check; |
| [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Targets    | &check; |
| [Langbehn and Steinicke, 2013](https://link.springer.com/referenceworkentry/10.1007/978-3-319-08234-9_253-1), Steer To Targets + Center    | &check; |

### Good Contribution

| Body Redirection  | Status   |
|:-------------- | -------------- |
| [Han et al., 2019](http://ieeexplore.ieee.org/document/8260974/), Instant    | &check; |
| Pseudo-Haptic   |     |
| [Lécuyer et al., 2000](https://doi.org/10.1109/VR.2000.840369), Swamp Illusion   | ?   |
| [Samad et al., 2019](https://dl.acm.org/doi/10.1145/3290605.3300550), Pseudo-haptic weight   | ?;   |

<!-- | World Redirection   | Status    |
|--------------- | --------------- |
| []()   | &cross;   |

| Interpolation   | Status    |
|--------------- | --------------- |
| []()   | &cross;   | -->

### Nice to Have

| Body Redirection  | Status   |
|:-------------- | -------------- |
| [Poupyrev et al., 1996](https://dl.acm.org/doi/10.1145/237091.237102), The Go-Go   | &check;   |
| World Redirection   |     |
| [Abtahi and Follmer, 2019](https://dl.acm.org/doi/10.1145/3290605.3300752), World-in-miniature   | &cross;   |
| Interpolation   |    |
| [Zhao et Follmer et al., 2018](https://dl.acm.org/doi/10.1145/3173574.3174118), Complex Boundaries   | &cross;   |
| Pseudo-Haptic   |     |
| [Argelaguet et al., 2013](https://doi.org/10.1145/2501599), Deformable materials   | &cross;   |
| [Kasahara et al., 2017](http://doi.acm.org/10.1145/3025453.3025962), Malleable Embodiement   | &cross;   |
| [Gomez Jauregui et al., 2014](http://ieeexplore.ieee.org/document/6777424/), Avatar Weight   | &cross;

## Outils de Visualisation

- Caméras réelle et virtuelle
- World Redir: Environnement en transparence (?)
- Trajectoires parcourues

### Logging
- Position et orientation de chaque élément à chaque instant (.csv)
  - Mains et/ou tête réelle et virtuelle
  - Environnement réel et virtuel
  - Objets importants à la technique d'interaction
- Profil de vitesse
	- Minimum jerk model
- Lecture à partir d'un fichier? Rejouer une scène à partir d'un csv (ou autre)


### Authors
Benoît Geslain (benoit.geslain@sii.fr, https://github.com/BenoitGeslain), Bruno Jartoux (bruno.jartoux@sii.fr, https://github.com/bjrtx)

### Copyright and licensing
Copyright (c) 2023 SII Société pour l’Informatique Industrielle


This project is licensed under the open-source MIT X11 license, see the LICENSE file.
In particular, you may not distribute any copy or substantial portion of this project
without the contents of the LICENSE file.
