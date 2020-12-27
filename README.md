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

This assembly models a Pose document that is opened in the editor and the operations you can perform on it. **All** changes to a document must go through the domain, which provides an API to the outside to do so. The UI of the editor will never change data by itself, it can't even reach that data without the domain API. The root object of the Domain is the ```Document```. The Document mainly contains a tree of bones and sprites (Nodes) and a list of Animations, which exist of PropertyAnimations and Keys. Behind each document is a ```DocumentData``` instance containing all data representing that Pose document. It is this ```DocumentData``` that is saved and loaded from/to a .pose file using Google's Protobuf system.

Some important concepts to understand in the Domain:

#### History (undo/redo)

All changes through the domain automatically are recorded by the History system (for undo/redo purposes). When a user deletes a Bone, all the child-bones are also removed. When we want to undo that delete, all child-bones need to be restored too. We need to store the combined information of all nodes that were deleted and store it as a single item in the History system. To solve the history tracking of this (and other) complex operations in a generic way, we define two layers of operations that can be performed on a document: 
* Events aka atomic operations: these represent the simplest, individual changes on the domain (a name change, a value change, a single node delete). These are stored as *Events* (eg. AnimationRenamedEvent). Each *Event* in the Domain assembly stores the necessary data and logic to both *do* and *undo* that atomic operation.
* User operations: these trigger (sometimes complex) algorithms that perform a compound set of one or more atomic operations (for instance deleting a Bone and all its children). When a user operation is started, a UnitOfWork is created. All atomic operations triggered during the algorithm are recorded onto that *UnitOfWork*. After the algoritm finishes, the list of atomic operations recorded on the UnitOfWork is stored into the History system as a single set of changed. It can then be used for undoing or redoing the complex chain of changes that the single user action caused to occur.

#### MessageBus

The domain emits Messages through a central MessageBus. To keep code cleanly separated, the different parts of the editor application (eg. the various visualisations of data throughout the UI) can subscribe to certain change-messages sent from the domain. They can then react to that message any time it occurs.

### Events vs Messages

Do not confuse Events with Messages: an *Event* represents a user changing the document. A *Message* represents the actual document change so the UI can update itself when that change happens. We need to separate these two concepts because a single *Event* can cause many *Messages* if the user choses to undo/redo that same *Event* over and over again. So, the data keeps on changing back and forth, forcing the UI to change again and again, but all that happens from reapplying the same Event over and over. So, the UI listens for Messages (actual data changes), while Events represent user intent in the History system. In fact, Events are internal classes (in the domain assembly), the UI cannot even see them.

### Pose.Domain.Editor assembly

This contains a layer on top of the Domain representing a central state and API to commandeer the editor's state and the document that is loaded within. The Pose.Domain assembly represents only a Pose document, it doesn't care about loading and saving those documents, or things like selecting objects in the editor. These things are modeled here, in the Pose.Domain.Editor. For the UI, the Editor is the single point of contact for performing changes on the document. So, the UI cannot directly address the domain, it must use the API of the Editor in this assembly.

The Editor is also responsible for creating a UnitOfWork (for the History system) and passing it along with API calls to the Pose.Domain assembly.

### Pose assembly

This assemlbly contains the UI (WPF) and is the glue that combines all parts of the application. It uses MVVM, but because MVVM has nasty downsides for more complex applications I use some deviations: to distinguish between user-initiated changes and code-initiated changes of properties, I tend to use .NET events when a change originates from user interaction instead of relying on the twoway binding.

* Folder 'Controls' contains some custom WPF controls
* Folder 'Panels' contains all panels of the editor (each visual part of the editor is called a Panel)
* Folder 'SceneEditor' contains the 2D viewport that shows and edits the visual scene. It uses WPF's Viewport3D which is a fairly thin layer over DirectX.
* Folder 'Shell' is the visual root of the application, splitting the main window in sections on which panels (from folder Panels) are positioned.
* Folder 'Startup' contains the bootstrapper and IoC container setup. If you don't know what that is, you can probably ignore it most of the time. (I do :) )
* Folder 'Themes' is WPF's styling etc for the custom controls used by the editor.
    

