                             Cryptnos for Windows
                                Version 1.3.3
                              Source ReadMe File

                            Jeffrey T. Darlington
                                July 14, 2015
                           http://www.cryptnos.com/

Cryptnos is a multi-platform, Open Source application for generating strong, pseudo-random passwords using cryptographic hashes.  It combines a unique "site token" such as a website domain name with a master password and runs this data through a cryptographic hash algorithm to produce a password that is unique, lengthy, seemingly random yet completely repeatable.  Unlike similar products, however, it is exceedingly flexible.  It is not a browser plugin, so it can be used with other applications outside the Web.  It provides unparalleled versatility by letting you specify the cryptographic hash to use, how many iterations of the hash to perform, what characters to include, and how long the final password should be.  Best of all, it is exceedingly secure.  Your master and generated passwords are *NEVER* stored, and the parameters to recreate your passwords are stored in an encrypted form.

Cryptnos is currently available for Microsoft Windows via the .NET Framework 2.0, as well as for Google Android powered devices.  A version for pure Java is under development, as well as an HTML/JavaScript online version that should work with any browser.  This ReadMe file is meant to accompany the Windows/.NET source code and is specific to that project.

BUILDING CRYPTNOS FOR WINDOWS
=============================

This source distribution for Cryptnos for Windows is a Microsoft Visual Studio 2012 Windows Application project.  Although it was originally built in Visual Studio 2012, you should be able to open and compile it in Express editions without any problem.

If you've downloaded our source and built it yourself in the past, you may remember seeing a couple of Windows batch files that were used in tagging Subversion revision numbers into the application version numbers.  With our move to GitHub and git, these files are no longer necessary and have been removed from the project; you can simply load up the solution and build it as-is.  If this is your first time building WinHasher yourself, you can safely ignore this and move along.  (This is not the disclaimer you were looking for....)

Once Cryptnos is built, make sure to copy the XML schema files (the .XSD files) in the root of the source tree into the same location as the binaries.  If Cryptnos cannot find these files, the update checker and import/export mechanisms will fail because Cryptnos will not be able to validate the XML used in these files.


BUILDING THE CRYPTNOS INSTALLER
==============================

The Cryptnos installer is built using Inno Setup 5.  I tend to use ISTool, which comes as an optional install with Inno Setup, to make writing the installer code a bit easier.  However, ISTool is not necessary for building the installer; the script should run in Inno Setup just fine.  You will need to build Cryptnos first to generate the executable before executing the script.  Make sure to check the paths within the script and modify them to fit your building environment.  A lot of paths in the script are relative to the setup script, but some are hard-coded and will need to be updated.

Note that Inno Setup no longer supports older versions of Windows.  Technically, WinHasher should build and run on any system that supports .NET 2.0; however, Inno Setup restricts us to "every Windows release since 2000".  Officially, we'll only support Windows XP and higher.

If you downloaded an "official" source archive, you can ignore this step, as the installer source is not distributed with the source archive.

