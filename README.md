# pose

A free 2D skeleton-based animation tool aimed at indie gamedevs. Use it for anything you like, also commercial, closed source projects. Crediting 'Pose' is appreciated but not mandatory.

![pose-screenshot](https://github.com/thomasvt/pose/blob/main/Manual/Pose-screenshot.png)

Create 2D animations by splitting your game asset into (body-)parts, adding a hierarchy of bones and animating individual bones using keyframes. After creating one or more animations for your game asset, you can import and run those animations in your game by using a runtime. (currently, only MonoGame)

Pose is in full development: the functional core is done, so it's possible to make animations using translation and rotation. 

Many features are still absent, but things are moving, so you may want to keep an eye out for updates. If you think you have feature suggestions or bugs that are not already on [the project's planning](https://trello.com/b/yuuP2bdf/pose), feel free to raise issues. Hint: the cleared your description, the more chance you have of getting my attention :)

There is a single runtime, which is for MonoGame (https://www.monogame.net). For programmers, porting that runtime's code to your game engine of choice should not be too difficult. It's in the runtimes folder of the code.

As the editor still needs a lot of work in the programming department, there are no manuals yet. That will come once I consider the editor to be more ready for public use. But, a lot of effort is put into the intuitivity of the application, so the learning curve hopefully is not steep when you're familiar with the concept of rigged animation. 
