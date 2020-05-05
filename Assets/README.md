# Card Game Simulator

This is a simple solitaire card game simulator. It provides the following capabilities:

1. Drag and drop
2. Flip
3. Re-insert card into deck (causes automatic shuffle)


## How to install

You need a licensed copy of Unity, version 2019.3.12 at the minimum.

Download the zipped file of this entire repository and unpack it somewhere.

Use Unity Hub to point to that location.

Depending on your operating system, you may need to update the build target. Accept the request.

Use Unity to run the game

## How to interact with the game

To deal a card from the deck, click on the deck and drag. The "top" card will be dealt and will move with the mouse.

To flip a card (over or back), hold the mouse over the card and press "f".

To put a card back into a deck, move the card so it overlaps with the deck and press "r".
The card will be flipped to back-face, and the deck will shuffle itself.

## Prefab internals

The game is made up of Card Decks

Each Card Deck has a number of Cards

Each Card has a Front and a Back image

Tokens just have a sprite handler for their image.

Cards and tokens have drag-and-drop capability, deck has deal and replace+shuffle capability.

### How to build a new game

1) Add a new scene for the game
2) Connect that scene to the main menu (see MenuSceneController.cs for how)
3) Copy in all your card, token and board assets.
4) Use the Sprite Editor to split up the assets into sprites (e.g. card fronts, card back)
5) Create a new empty object with a Box Collider 2D and Card Behaviour components.
6) Under the empty object, create "front" and "back" sprite renderers for the card front and the card back
7) Link those renderers to the Card Behaviour script.
8) Set the collider for the top game object to cover the size of the displayed cards.
9) Save the object as a prefab.

Repeat steps 5-8 for each card. Listen to some nice music while you do it, because it's quite tedious.

To make a deck:
1) Create a new empty object, add Deck Behaviour.
2) According to the game's user guide, determine the # of cards in the deck.
3) Drag and drop card prefabs into the array.
4) Save the deck as a prefab.

When all this is done, you can lay out the board and away you go!


## Future Enhancements

- Add a zoom and pan capability to the camera so can have bigger boards and closeups on cards.
- Have some way to track a deck when it runs out, so we can put cards into an empty deck.
- Refactor the movement code, as it's repeated almost exactly between Token and Card.
- Add a "do you want to exit" confirmed return to the menu screen when the user pressed Escape.
