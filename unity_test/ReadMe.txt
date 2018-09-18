Version de Unity utilisée : 2017.3.0

Le projet a besoin de l’assembly System.Drawing en version 4.5 pour fonctionner
sur ma machine. De ce fait Unity est paramétré en mode expérimental pour
utiliser .NET 4.5 . Cela a été nécessaire parce que l’assembly System.Drawing
compatible .NET 3.0 sur ma machine était seulement en version 2.0 et posait
problème. System.Drawing fait par ailleurs usage d’une bibliothèque partagée
libgdiplus qu’il a également été nécessaire d’ajouter.

J’ai observé des échecs lors des premiers chargements de textures après avoir
lancé Unity, indépendament du fait que les textures soient en cache ou pas.

Je copie les pixels de mon image PNG dans un tableau de byte pour la passer
à Texture2D.LoadRawTextureData . Il y a peut être un moyen de le faire sans
copie en utilisant directement le IntPtr renvoyé par Bitmap.GetHbitmap .

Par ailleurs j’utilise le ThreadPool du runtime pour paralléliser le décodage
des textures. Si un certain nombre de modules d’un jeu en faisaient un usage
suffisement intensif il pourrait être nécessaire d’ajouter une surcouche ou
d’implémenter des pools personalisés pour s’assurer qu’un module qui planifie-
rait de nombreuses tâche à un moment donné ne s’accaparerait pas l’intégralité
des threads du pool alors que les autres modules sont bloqués.
