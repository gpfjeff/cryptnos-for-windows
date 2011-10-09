                             Cryptnos for Windows
                                Version 1.3.0
                              Source ReadMe File

                            Jeffrey T. Darlington
                               October 8, 2011
                           http://www.cryptnos.com/

Cryptnos is a multi-platform, Open Source application for generating strong, pseudo-random passwords using cryptographic hashes.  It combines a unique "site token" such as a website domain name with a master password and runs this data through a cryptographic hash algorithm to produce a password that is unique, lengthy, seemingly random yet completely repeatable.  Unlike similar products, however, it is exceedingly flexible.  It is not a browser plugin, so it can be used with other applications outside the Web.  It provides unparalleled versatility by letting you specify the cryptographic hash to use, how many iterations of the hash to perform, what characters to include, and how long the final password should be.  Best of all, it is exceedingly secure.  Your master and generated passwords are *NEVER* stored, and the parameters to recreate your passwords are stored in an encrypted form.

Cryptnos is currently available for Microsoft Windows via the .NET Framework 2.0, as well as for Google Android powered devices.  A version for pure Java is under development, as well as an HTML/JavaScript online version that should work with any browser.  This ReadMe file is meant to accompany the Windows/.NET source code and is specific to that project.

BUILDING CRYPTNOS FOR WINDOWS
=============================

This source distribution for Cryptnos for Windows is a Microsoft Visual Studio 2010 Windows Application project.  Although it was originally built in Visual Studio 2010, you should be able to open and compile it in Visual C# 2010 Express without any problem.  (In fact, I tend to use Visual C# 2010 Express for official builds.)  Note, however, that there are a few local modifications you may need to make to the files before building.

If you checked out the source from the source repository, you will find two Windows batch files, "new_revision_tag.bat" and "SubWCRev_batch.bat".  These scripts are actually Perl scripts (written for ActiveState's Active Perl) encased in a batch wrapper and are used to add the SVN revision number and copyright date to the officially builds.  "new_revision_tag.bat" is intended to be run as a pre-commit hook script and updates a random "tag" in a comment inside the "template" files to force SVN to always update the templates before a commit.  "SubWCRev_batch.bat", its companion script, is run as a post-commit and post-update script which runs SubWCRev.exe, which parses the templates and adds the revision and copyright date information.  If you wish to take advantage of these scripts, replace the $workingpath variable value with the path to the root of your working copy.  If you're running this on Windows, make sure to escape your back-slashes; if you're running it on a *NIX setup, remove the Windows batch information, replace the "shebang" line with the correct path to your Perl executable, and tweak the path strings in the @templates array with the correct path separators (forward slashes instead of back-slashes).  You will also need to configure your local SVN setup to execute these scripts on the appropriate hooks.

If you do not wish to take advantage of the hook scripts, look for the *.template files throughout the source tree.  Rename them to remove the ".template" extension, then edit them to replace the $WC*$ variables.

If you downloaded an "official" ZIP archive of the source rather than checking the code out of the repository, you can ignore the above comments about the hook scripts.  The "official" source archives have the scripts and templates removed and the templated files will already have the correct revision and copyright information in place.  It should be ready to build as-is.

Once Cryptnos is built, make sure to copy the XML schema files (the .XSD files) in the root of the source tree into the same location as the binaries.  If Cryptnos cannot find these files, the update checker and import/export mechanisms will fail because Cryptnos will not be able to validate the XML used in these files.


BUILDING THE CRYPTNOS INSTALLER
==============================

The Cryptnos installer is built using Inno Setup 5.  I tend to use ISTool, which comes as an optional install with Inno Setup, to make writing the installer code a bit easier.  However, ISTool is not necessary for building the installer; the script should run in Inno Setup just fine.  You will need to build Cryptnos first to generate the executable before executing the script.  Make sure to check the paths within the script and modify them to fit your building environment.  A lot of paths in the script are relative to the setup script, but some are hard-coded and will need to be updated.

Note that the installer script is templated using the same commit hook scripts mentioned in the "Building Cryptnos" section.  If you checked out the source from the repository, you may need to set up the hook scripts or otherwise manually tweak the files.

If you downloaded an "official" source archive, you can ignore this step, as the installer source is not distributed with the source archive.

