Lucas Jobert GD2 Devoir Roll A Ball

Nom du projet : Polarity Ball
Vue : top down, caméra fixe
Genre : Puzzle

Objectif : Contrôler une balle magnétique qui peut modifier sa polarité en appuyant sur une touche. Le but est de résoudre des puzzles afin d’ouvrir la porte qui mène au niveau suivant.

Mécanique de déplacement : Changement de la logique de déplacement : je suis passé en « Add force » pour un côté plus physique qui collait mieux au thème du jeu. J’ai fait en sorte de pouvoir paramétrer la friction, l’accélération, la max speed et la vitesse de « virage » afin de pouvoir obtenir un rendu contrôlable et arcade.
Cet ajout m’as aussi permis d’ajouter de nouveau mode de déplacement.
Ce n’est pas intégré directement au joueur mais on peut par exemple se projeter ou s’attirer vers des zones magnétiques.

Score : Deux types de collectible : le 1er ajoute un faible nombre de points au score
Le 2eme Ajoute un compteur d’étoile et un gros nombre de points au score.
Le résumé s’affiche dans le menu de fin de level.
Les étoiles, qui n’ont malheureusement pas eu d’assets par manque de temps, sont plus dur a obtenir et nécessite de prendre un chemin alternatif plus dangereux. Il propose une forme de challenge supplémentaire.

Objets Magnétiques et portes : Des cubes ayant une physique et une polarité propre. Ils réagissent aux champs magnétiques du joueur et des autres objets.
Ils servent à alimenter des zones d’énergies permettant d’ouvrir des portes. 
Les zones d’énergies ont-elles aussi une polarité, elle doit donc être opposé à celle du cube pour recevoir l’energie. Si ce n’est pas le cas, elle le repousse.

Autres types d’objets : 
- Ancre magnétique : Objet fixe sans physique émettant un champ magnétique. Il peut accueillir un script pour le déplacer d’un poids A à B en boucle. Je m’en suis servi notamment pour créer des puzzles ou il faut s’aimanter dessus pour passer une zone de vide. Ou bien pour se projeter dans une direction en ligne droite.

- Zone magnétique simple : plus ou moins la même fonction mais beaucoup plus aléatoire dans la direction ou l’on se projette. Elle peut servir à bloquer certaines zones pour des cubes magnétiques.

- Interrupteur magnetique : Il sert à inverser la polarité d’une Ancre magnétique.

- Laser : Il détruit le joueur s’il passe dedans en ayant la même polarité que lui. 



Level Design et Ui : Un menu principal avec boutons play et quit. Quatre level bien distinct ayant pour but de montrer les différentes mécaniques en poussant un peu plus à chaque level. Un menu de fin de level pour passer au suivant, restart, ou quitter le jeu.

Déroulement et comment j’ai fonctionné : J’ai majoritairement utilisé une IA pour m’aider à coder le projet. N’étant pas un excellent développeur et qui plus est en ayant commencé le projet en retard, je pensais que c’était la meilleure solution pour avancer assez vite vers quelque chose de satisfaisant selon mon idée. 
Malgré cela j’ai quand même dû comprendre et lire attentivement les scripts afin de pouvoir l’orienter vers ce que je voulais au niveau des mécaniques.
Il y a certains codes que j’ai demandé que je ne saurais pas vous expliquer car je n’en ai pas pris le temps, notamment des codes pour appliquer un material automatiquement selon la polarité de l’objet. J’ai considéré ces genres de scripts comme des outils pour gagner du temps mais dont j’aurais pu me passer.

Pour ce qui est du déroulement, j’ai pris beaucoup de retard notamment du au fait que j’ai commencé à travailler trop tard sur le projet.
En effet avant cela j’étais concentré sur un projet personnel sur unreal qui dure depuis la fin d’année de GD1. Alors c’était difficile pour moi de lâcher le projet en cours pour basculer sur un autre moteur de jeu.



Je suis conscient que c’est entièrement ma faute et que j’aurai dû mieux anticiper le temps que ça me prendrait.
Je tiens donc à m’excuser pour le projet que je vous rends, qui est très incomplet.

Je pense malgré tout avoir appris pas mal de chose et être plus à l’aise sur unity grâce à ce projet, mais il est clair que je suis assez déçu et frustré de ne pas avoir pu vous proposer un projet plus abouti.
Je pense aussi que mon scope n’était pas forcément adapté, mais j’ai préféré suivre mon idée afin d’expérimenter et de voir comment pourrais se dérouler un projet qui aborde la physique sur ce moteur de jeu.

Merci,
A bientôt.





