# Index
A tool for browsing and extracting assets from Halo 2 Anniversary.

Index will NOT open files from the original Halo 2. It only works with Saber Interactive's files (e.g. *.pck).

# Credits
Haus, zatarita, sleepyzay, Unordinal, and a whole bunch of other people we forgot to 7hank.

# Tutorial Video
Click the thumbnail below to view.

[![Tutorial Video](http://img.youtube.com/vi/hfji1ik2wFk/0.jpg)](http://www.youtube.com/watch?v=hfji1ik2wFk "How to rip Halo 2 Anniversary Models, Textures, and Maps with Index")

# Building
The LibH2A library uses .NET Standard 2.0, so it will work with both .NET Framework and .NET Core. This library reads Saber Interactive's files. It can easily be plugged into any .NET project.

The H2AIndex project targets .NET 6 and uses WPF to drive its frontend. It is designed for computers running 64-bit builds of Windows. Building the program should be straightforward after restoring the NuGet packages.
