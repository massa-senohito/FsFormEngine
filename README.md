# FsFormEngine
Template for winform in f# with directinput

To create game in F#, often Unity would be chosen.
But it needs a long step to debug.
1. Build dll and add some library to Library folder.
2. Attach VisualStudio to Unity.
3. Run in Unity.
4. Set Breakpoint in F# code.

I want to debug quickly. In addition, I don't need advanced expressions such as AO. 

So I considered using Winforms.
It has Buttons and TextBox, and has enough features to create a temporary UI.
Winforms can use joystick as input, if you import SharpDX.DirectInput.
After testing with this, I think that if you use it as a Unity library, you can improve the efficiency of development. 
