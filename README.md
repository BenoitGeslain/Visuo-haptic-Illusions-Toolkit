# Visuo-haptic Illusions

TODO: Ajouter description

## Contribuer

Nous sommes nombreux à travailler sur le projet, pour que le code soit toujours formaté de la même manière et reste facile à lire, on va utiliser le format [The One True Brace Style](https://en.wikipedia.org/wiki/Indentation_style#Variant:_1TBS_(OTBS)) pour structurer le code et le [camelCase](https://fr.wikipedia.org/wiki/Camel_case) pour nommer les variables. Le One True Brace Style n'est pas le format que Unity et VS utilise par défaut donc essayez de faire attention. Si vous êtes sur VSCode, je peux vous montrer comment le configurer pour que ça soit le défaut.

Le rendu final de votre participation au projet est un rapport à réaliser au fur et à mesure que vous avancer. Toutes les informations sont disponibles [ici](../../wikis)

Je vous recommande de créer une branche avec votre nom ou prénom à partir de la branche qui vous a été assignée et de merge régulièrement.

Voir le [wiki](../../wikis) pour plus d'informations sur les tâches à réaliser et les instructions d'installation Unity

## Techniques d'interaction

### Must Have

| Body Redirection  | Status   |
|:-------------- | -------------- |
| [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), Body Warping    | &cross; |
| [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), World Warping    | &cross; |
| [Azmandian et al., 2016](https://doi.org/10.1145/2858036.2858226), Hybrid Warping    | &cross; |
| [Han et al., 2019](http://ieeexplore.ieee.org/document/8260974/), Continous    | &cross; |
| [Cheng et al., 2017](http://doi.acm.org/10.1145/3025453.3025753), Sparse Haptics   | &cross;   |
| [Geslain et al., 2022](https://doi.org/10.1145/3531073.3531100), 2<sup>nd</sup> order polynomials   | &cross;   |
| World Redirection   |     |
| [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Over Time Rotation    | &cross;   |
| [Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/), Translational    | &cross;   |
| [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Rotational <br />[Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/)    | &cross;   |
| [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Curvature <br />[Steinicke et al., 2008](http://ieeexplore.ieee.org/document/4741303/)    | &cross;   |
| [Razzaque et al., 2001](https://diglib.eg.org:443/xmlui/handle/10.2312/egs20011036), Hybrid    | &cross;   |
| Interpolation   |     |
| [Kohli et al., 2010](), Redirected Touching<br />[Kohli, 2013](https://doi.org/10.1109/3DUI.2010.5444703)   | &cross;   |
| Pseudo-Haptic   |     |
| [Lécuyer et al., 2000](https://doi.org/10.1109/VR.2000.840369), Spring stiffness   | &cross;   |
| [Gomez Jauregui et al., 2014](http://ieeexplore.ieee.org/document/6777424/), Weight   | &cross;   |

### Good Contribution

| Body Redirection  | Status   |
|:-------------- | -------------- |
| [Han et al., 2019](http://ieeexplore.ieee.org/document/8260974/), Instant    | &cross; |
| Pseudo-Haptic   |     |

| [Lécuyer et al., 2000](https://doi.org/10.1109/VR.2000.840369), Swamp Illusion   | &cross;   |

<!-- | World Redirection   | Status    |
|--------------- | --------------- |
| []()   | &cross;   |

| Interpolation   | Status    |
|--------------- | --------------- |
| []()   | &cross;   | -->

### Nice to Have

| Body Redirection  | Status   |
|:-------------- | -------------- |
| [Poupyrev et al., 1996](https://dl.acm.org/doi/10.1145/237091.237102), The Go-Go   | &cross;   |
| World Redirection   |     |
| [Abtahi and Follmer, 2019](https://dl.acm.org/doi/10.1145/3290605.3300752), World-in-miniature   | &cross;   |
| Interpolation   |    |
| [Zhao et Follmer et al., 2018](https://dl.acm.org/doi/10.1145/3173574.3174118), Complex Boundaries   | &cross;   |
| Pseudo-Haptic   |     |
| [Argelaguet et al., 2013](https://doi.org/10.1145/2501599), Deformable materials   | &cross;   |
| [Kasahara et al., 2017](http://doi.acm.org/10.1145/3025453.3025962), Malleable Embodiement   | &cross;   |

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