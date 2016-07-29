# Fable on React Native demo

This is a very early prototype for React Native development in F# using Fable.

If you want to try it out then please do the following:

### Make sure React Native works

* Install the [VS Code extension for React Native](https://github.com/Microsoft/vscode-react-native)
* Go to https://facebook.github.io/react-native/ and do all steps of the [getting started tutorial](https://facebook.github.io/react-native/docs/getting-started.html#content) 
* At this point you should have the default Javascript React Native "Hello World" running

### Running F# code on React Native

* Pull latest master
* Run "npm install" from command prompt
* Open Android Simulator
* Open VS Code
* Press F5
* Wait until device is ready and you will see:

[![error message](https://cloud.githubusercontent.com/assets/57396/17221919/1053c422-54f6-11e6-9b15-0701074a7b33.png)

(this is still an issue and discussed in [https://github.com/Krzysztof-Cieslak/fable-react_native-demo/issues/3]. We will get rid of this.)

* Press Ctrl-Shift-B
* Go to android deice and press r+r

![Device Running](https://cloud.githubusercontent.com/assets/57396/17221985/6568f806-54f6-11e6-8913-e3f4230094d3.png)

* Go to the F# source in VS Code make a change and save the file
* Go to android deice and press r+r - change should be visible