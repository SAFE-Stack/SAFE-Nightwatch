# Fable on React Native demo

This is a very early prototype for React Native development in F# using Fable.

If you want to try it out then please do the following:

### Make sure React Native works

* Install the Ionide [VS Code extenstion for F#](https://github.com/ionide/ionide-vscode-fsharp)
* Install the [VS Code extension for React Native](https://github.com/Microsoft/vscode-react-native)
* Go to https://facebook.github.io/react-native/ and do all steps of the [getting started tutorial](https://facebook.github.io/react-native/docs/getting-started.html#content) 
* At this point you should have the default Javascript React Native "Hello World" running

### Running F# code on React Native

* Pull latest master
* Run "npm install" from command prompt
* Open Android Simulator
* Open VS Code
* Press F5 to start debug
* Go to android device simulator and you should see something like

![Device Running](http://www.navision-blog.de/images/nightwatchapp.gif)

* Go to VS Code and hit Ctrl+Shift+B this will activate fable watch 
* Go to the F# source in VS Code make a change and save the file
* Fable watch should pick up this change automatically and compile it
* Go to android device simulator and press r+r - change should be visible

This could look like:

![Hot loading](http://www.navision-blog.de/images/hotloading.gif)