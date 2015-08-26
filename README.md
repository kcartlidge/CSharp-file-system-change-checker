# C# file system change checker

The .Net framework includes file watchers. However I have recently discovered that sometimes multiple events are fired with those watchers. For example Notepad fires twice; once to save the file and again to update it's attributes.

This means that if you rely on the file watcher in some situations you get double counting, race conditions or locks.

This is an alternative. Instantiate the class and call Check with a folder and file mask. On subsequent calls (probably via a timer or thread) when you make the same call it will return a state which reveals what changes have ocurred.

Note that this is working code but is a skeleton which only returns a flag state. You will need to expand the innards if you wish to expose the actual individual changes. I've done that for one of my own projects and it is a trivial change.
