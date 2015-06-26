# Disabling the Update Check #

_**NOTE:** The bug referenced in this article should now be fixed. If you previously disabled the update check, please update to version 1.2.0 and re-enable the check by deleting the `DisableUpdateCheck` registry key created below. If you continue to have problems, please add a comment in the Issue thread referenced below and we'll reopen the ticket._

We have a report of what seems to be a [major bug](http://code.google.com/p/cryptnos-for-windows/issues/detail?id=1) in the update notification code in the recently released Cryptnos for Windows 1.1. If after installation you get a rather cryptic "Object reference not set to an instance of an object" error that prevents Cryptnos from ever starting, you've likely encountered it. It does not seem to be affecting every user. It's going to take me a while to take a look at it and fix it, but we have found a way to bypass the error so you can get Cryptnos 1.1 to work.

A few warnings up front: This workaround will completely disable the update checking code until it is removed. You will have to manually check for updates while the workaround is in effect. You will also have to manually remove the workaround later to disable it. It requires registry hacking by hand. Modifying the Windows registry can be dangerous if you don't know what you're doing. If you are not comfortable making this change, please uninstall Cryptnos 1.1 and fall back to Cryptnos 1.0 until an official fix can be released. Note that Cryptnos 1.0 is unaffected by this issue.

  1. Open the registry editor by going to the Start button and clicking Run.
  1. In the Run dialog, type `regedit` and click OK. The registry editor will open.
  1. Navigate to `HKEY_CURRENT_USER\Software`.
  1. Look for the registry key folder `GPF Comics`. If it exists, open it. If not, create it. Note that this is a folder, not a key/value pair.
  1. Under `GPF Comics`, look for the registry key folder `Cryptnos`.  If it exists, open it; if not, create it.
  1. Inside this `Cryptnos` folder, right-click to bring up a context menu. Select New -> DWORD Value.
  1. Name this new key `DisableUpdateCheck`.
  1. Double-click the newly created key. Give it the value of 1 (the number one). Click OK.
  1. Close the registry editor.
  1. If you have multiple logins on this computer, this hack will only affect the currently logged in user. You will need to apply this change to all users on the system to make Cryptnos usable for those users.

This should bypass the update check on start-up and Cryptnos should load successfully. Note that when we finally release the fix, you will need to manually remove this registry key value to re-enable the update check.