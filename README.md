# Reclaimer.Saber3D
A plugin for Reclaimer that adds support for Saber3D Engine Files

# Credits
Haus, zatarita, sleepyzay, Unordinal, and a whole bunch of other people we forgot to 7hank.

# Building
Reclaimer uses .NET Framework 4.6.2.
The LibH2A library uses .NET Standard 2.0, so it will work with both .NET Framework and .NET Core.

The solution expects the Reclaimer source code to be in an adjacent folder.
Your directory structure should look something like this:

```
/Code
  /Reclaimer
  /Reclaimer.Saber3D
    Reclaimer.Saber3D.sln
```

If this isn't working, just remove the dependencies from the project and add new references to the Reclaimer projects.
