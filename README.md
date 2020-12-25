# pose

A free 2D skeleton-based animation tool aimed at indie gamedevs. Use it for anything you like, also commercial, closed source projects. Crediting 'Pose' is appreciated but not mandatory.

![pose-screenshot](https://github.com/thomasvt/pose/blob/main/Manual/Pose-screenshot.png)

Create 2D animations by splitting your game asset into (body-)parts, adding a hierarchy of bones and animating individual bones using keyframes. After creating one or more animations for your game asset, you can import and run those animations in your game by using a runtime. (currently, only MonoGame)

## Pose is under construction

Pose is in full development: the functional core is done, so it's possible to make animations using translation and rotation. But be advised that it a lot of things are still absent.

Development is moving, though, so you may want to keep an eye out for updates. See [the project's planning](https://trello.com/b/yuuP2bdf/pose) to know what's going on. If you think you have feature suggestions or bugs that are not already on the planning (please double check!), feel free to raise issues on GitHub. Hint: the cleared your description, the higher your chances on getting attention :)

## Runtimes

There is a single runtime, which is for MonoGame (https://www.monogame.net). For programmers, porting that runtime's code to your game engine of choice should not be too difficult. It's in the runtimes folder of the code.

## Manual

As the editor still needs a lot of work regarding important basic editor features, there is no use in spending my time on manuals. But, a lot of effort is put into the intuitivity of the application, so, hopefully, the learning curve is not too steep when you're familiar with the concept of rigged animation. 

## Programming/technical overview

If you want to contribute, or trace down a bug, I would be very grateful. To get you started, the following is a highlevel overview of the software architecture. But feel free to reach out to me if you have questions.

The editor is created in Visual Studio, uses .NET Core and WPF for UI.

### Pose.Domain assembly

This assembly models a Pose document that is opened in the editor and the operations you can perform on it. **all** changes to a document must go through the domain, which provides an API to do so. The UI of the editor will never change document data by itself, it's even impossible to reach that data without the domain. The root object of the Domain is the ```Document```, which contains a ```DocumentData``` containing all data representing a single Pose document. It is this ```DocumentData``` that is saved and loaded from/to a .pose file.

#### History (undo/redo)

All changes through the domain automatically are recorded by the History system (for undo/redo purposes). When you delete a Node in the hierarchy, all its possible children are also removed. To make this undoable, we need to record all data involving these children. Therefore, we need to store this information in a list of all nodes that were deleted and store it in the undoable item. To solve this problem in a generic way, we define two layers of operations that can be performed on a document: 
* simple, atomic operations: these change simple things, and store the data to undo themselved. These are stored as Events. Each Event in the Domain assembly stored the data and logic for doing and undoing the atomic operation.
* user operations: these trigger (sometimes complex) algorithms that perform one or more atomic operations that are recorded onto a *UnitOfWork*. After the algoritm finishes, the UnitOfWork contains a list of all atomic operations that were performed. This is the list that is stored into the History system and can be undone or redone upon user request.

#### MessageBus

The domain emits Messages through a central MessageBus. The different parts of the editor application (eg. the UI panels) can subscribe to certain messages they are interested in, so they can update the data they show on screen when things change in the domain (the document).

Do not confuse Events with Messages: Events are created when a user changes something, the *Event* is recorded in History and also applied to the domain. Applying the event causes the domain to change, which publishes a *Message* for the UI to update itself. In constrast: when a user undoes and then redoes a user action, there is no recording of a new Event because we are redoing an already existing event. So, the event is reapplied to the domain, causing changes to the domain, causing messages to be raised so the UI can update itself. 

So, the sum up: *events* get recorded, once, when a user performs actions on the document; while *messages* get raised when the domain is changed by playing, undoing, or redoing events. Therefore, Event classes must be internal, they should not be visible from the UI assembly.

### Pose.Domain.Editor assembly

This contains a layer on top of the Domain representing a central state and API to commandeer the editor's state and the document that is loaded within. The Pose.Domain assembly represents only a Pose document, it doesn't care about loading and saving those documents, or things like selecting objects in the editor. These things are modeled here, in the Pose.Domain.Editor. For the UI, the Editor is the single point of contact for performing changes on the document. So, the UI cannot directly address the domain, it must use the API of the Editor in this assembly.

The Editor is also responsible for creating a UnitOfWork (for the History system) and passing it along with API calls to the Pose.Domain assembly.

### Pose assembly

This assemlbly contains the UI (WPF) and is the glue that combines all parts of the application. It uses MVVM but, because MVVM has nasty downsides for more complex applications: to distinguish between user-initiated changes and code-initiated changes of properties, I often use events when a change originates from user interaction instead of twoway binding. 
    * Folder 'Controls' contains some custom WPF controls
    * Folder 'Panels' contains all panels of the editor (each visual part of the editor is called a Panel)
    * Folder 'SceneEditor' contains the 2D viewport that shows and edits the visual scene. It uses WPF's Viewport3D which is a fairly thin layer over DirectX.
    * Folder 'Shell' is the visual root of the application, splitting the main window in sections on which panels (from folder Panels) are positioned.
    * Folder 'Startup' contains the bootstrapper and IoC container setup. If you don't know what that is, you can probably ignore it most of the time. (I do :) )
    * Folder 'Themes' is WPF's styling etc for the custom controls used by the editor.
    

