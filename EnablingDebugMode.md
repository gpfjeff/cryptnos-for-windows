_**Note:** This hack is no longer required. Starting with version 1.2.1, a checkbox has been added to the Advanced Settings dialog to enable and disable Debug Mode without having to manually modify the registry. If you need Debug Mode for any reason, please upgrade to 1.2.1 or later and use this setting rather than performing the manual tweak below._

# How to Enable Debug Mode in Cryptnos #

Cryptnos 1.1.1 and higher now has a "debug mode" that produces more robust but less user friendly error messages. This can help users troubleshoot problems when they are encountered and provide additional feedback to the developers. It is an entirely optional and undocumented (well, except for right here) "feature" and there is no user interface for turning it on or off. Debug mode is turned off by default and requires manual editing of the Windows registry to turn it on.

An up-front warning: Modifying the Windows registry can be dangerous if you don't know what you're doing. If you are not comfortable making this change, please do not attempt it.

  1. Open the registry editor by going to the Start button and clicking Run.
  1. In the Run dialog, type `regedit` and click OK. The registry editor will open.
  1. Navigate to `HKEY_CURRENT_USER\Software`.
  1. Look for the registry key folder `GPF Comics`. If it exists, open it. If not, create it. Note that this is a folder, not a key/value pair.
  1. Under `GPF Comics`, look for the registry key folder `Cryptnos`.  If it exists, open it; if not, create it.
  1. Under `Cryptnos`, look for the registry key `DebugMode`.  If it exists, double-click it; if not:
    1. Right-click inside `Cryptnos` to bring up a context menu.
    1. Select New -> DWORD Value. Name this new key `DebugMode`.
    1. Double-click the newly created key.
  1. Give the key the value of 1 (the number one). Click OK.
  1. Close the registry editor.

If you have multiple logins on this computer, this hack will only affect the currently logged in user. To turn debug mode back off, repeat the above process and change the value of the `DebugMode` key to 0 (zero). (You may also delete this key if you wish; Cryptnos will recreate it and set it to zero if it is not present.)