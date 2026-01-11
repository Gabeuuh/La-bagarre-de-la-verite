# La-bagarre-de-la-verite

Jeu VR narratif centre sur une lettre a reconstituer. Des mots circulent devant le joueur, il doit saisir les bons et les placer sur les emplacements correspondants pour reveler la verite.

## Le jeu en bref
- Flux de mots a attraper et deposer sur des slots (SUJET, ACTION, LIEU, etc.).
- Les bons mots valident la lettre, les mauvais comptent comme erreurs.
- La narration guide le joueur vers le type de mot attendu.
- Quand tous les slots sont remplis, la sortie se debloque.

## Boucle de jeu
1. Le joueur s'assoit (interaction chaise) et le mini-jeu demarre.
2. Les mots defilent; le joueur en saisit un et le depose sur le slot.
3. Si le mot est correct, il est valide et la lettre se complete.
4. Une fois la lettre complete, l'environnement s'ouvre et le joueur peut sortir.

## Systeme d'erreurs
- Chaque mauvais mot ajoute une erreur et declenche un flash rouge.
- Au bout de 3 erreurs, la scene est reset.



## Scenes
- Scene principale: `Assets/Scenes/Scene_Gabin.unity`
- Scene de base: `Assets/Scenes/BasicScene.unity`

## Lancer la scene
1. Ouvrir le dossier `Bagarre` dans Unity.
2. Ouvrir `Assets/Scenes/Scene_Gabin.unity`.
3. Cliquer sur Play.
