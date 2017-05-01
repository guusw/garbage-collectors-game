## Garbage Collectors

#### Description
A 2D versus game where you try to collect planets or objects by moving them into your corner of the screen by pushing them with your space ship

![](https://i.imgur.com/hoPPCN3.png)

#### Various mechanics:
* 1/2 minutes rounds
* Astroids-style movement (no shooting)
* Collection area slowly sucks in contents, so they can be moved for a while after they enter
* New objects appear every so often
    * Single objects 
    * Arrays (objects but connected)
        * Each node still counts as a single object, either inside or outside the collection area
* Power-ups
    * Speed
* Mines - to push into the enemy's corner and blow everything away (or destroy?)
* Using Xenko class names for the planets and making them objects instead

#### Development requirements:

Can be done alone of expanded upon if there are more people

##### Extras:

* Audio
* Music


## TODO List (Sorted by priority)

* Create space-ship controls
    * No wrapping, this means the ship and all objects bounce from the walls
    * Controller support
    * Variable player
    * Teams
* Create objects
* Create collection areas
* Create game timer and scoring
* Text on objects (ui layer using projected world space positions)
* (optional) last-touched object state
* Random name using the names of xenko classes for each object
