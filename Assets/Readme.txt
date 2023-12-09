## NOTE
 - The game is controlled using a Xbox controller or a keyboard/mouse.
 - The starting scene is StartMenu, however all scenes loop back around.
 - Animations and player models are not final.

Description: The player moves around different environments trying to achieve some goal.
    These goals are either reaching a specific point or retrieving a number of items.

Known Bugs:
    - The camera will move left and right around the character, but not up or down.
    - The angles the player is able to ascend are very steep, so the player is able to scale extremely slanted walls.

Assumptions:
    - The player is able to open the game.
    - If this game is opened in the editor window, the packages "Cinemachine" and "Input System" are installed.
    - If this game is opened in the editor window, the project must build all 4 scenes in order to run.
        * The "StartMenu" scene
        * The "ForestLevel" scene
        * The "MountainLevel" scene
        * The "PortCityLevel" scene
    - When the game is on the StartMenu scene, the game is paused, or the player has collected all the bottles,
        the game will switch to a pause function and the player must use their mouse to navigate and interact with the UI.
    - When the player starts the game, they will look at the signs and follow the premade path laid out throughout the scene.
        Exploring is fine, there is an invisible wall preventing a player from falling off, but the intended path is designating by a path.


