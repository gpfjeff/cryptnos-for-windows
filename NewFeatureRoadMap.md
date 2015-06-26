# New Feature Road Map #

This page documents a few new features and enhancements I'm considering adding in future releases. Whether or not they get implemented depends on (a) user feedback (i.e. whether the feature is user requested or commented upon), (b) level of difficulty (some more complicated tasks may require more time to implement), and (c) time (I wear many hats, so development may take longer to implement as my other duties shift and change).

## "Remove Confusing Characters" option in password generation ##
**Status:** Considering, not started

**Level of Difficulty:** Moderate

**Implementation Time:** Significant; requires UI, registry, business logic, and import/export format changes

**Details:** After a good deal of thought, I'm considering adding a new password generation option to remove "confusing" characters from the generated password. For example, the number one is easily confused with both the lowercase L and capital I characters; removing all three would make retyping, memorization, and verbal conveyance of the password easier and less confusing. To keep the same level of entropy, these "confusing characters" should be replaced with symbols, which requires identifying symbols that are not commonly used as delimiters, wild cards, and other uses that might prove problematic. The current idea would be to implement this as a new checkbox/toggle separate from other options so it may be combined with other parameters. The default would have this setting turned off, which implements the current behavior.

This won't be an easy one, which is why I may not implement it. Adding a new parameter means changes to the business logic (SiteParameters class), registry (adding a new "column" to the data that defaults to null/false), UI (a new checkbox/toggle button), and import/export file format (XML schema changes with the corresponding element being optional to preserve old exports). This also requires the Android client to be upgraded with the same feature to make sure they are in sync. It's possible that other parameters may clobber the benefits of this setting; for example, changing the character class to "Alphanumerics only" eliminates the substituted symbols, effectively reducing the characters per slot from 62 to 56 (assuming six "confusing" characters were substituted). So this feature may or may not get implemented depending on whether or not users feel it is a worthwhile addition.

## Encrypt registry entries with random key from [Random.org](http://www.random.org/) ##
**Status:** Considering, not started

**Level of Difficulty:** Moderate

**Implementation Time:** Significant; requires UI, registry, and business logic changes

**Details:** Currently, the registry entries store parameter data encrypted using AES-256 with a key that includes a salt derived from the machine's and user's login name. This in theory makes the key unique per user/machine combination, but it certainly isn't ideal. Both the machine and user name are easily obtainable and, worse, changeable. If a user's login information is changed or the machine is renamed, the encryption key changes and the registry values can no longer be decrypted. This can be avoided in advance if the user exports their parameters before the change and re-imports them afterward, but not all users will have this luxury.

While I still think it was a good idea to encrypt the data with a unique user-specific key, it may not be the best long term solution. While these types of changes may not be common, they're certainly not insignificant. I've been toying with the idea of adding a small HTTP client class that fetches a string of truly random bytes from [Random.org](http://www.random.org/) and using that (or more likely that value hashed and salted multiple times) as the encryption key. This would offer a bit more security in that the encryption key is not based on possibly volatile and easily guessed data.

Known problems with this idea:
  * Obviously, the HTTP client must be implemented, including for good measure code to check the bit quota to prevent abuse. (I've already done most of this as a coding exercise anyway.)
  * The random bytes, once fetched, must be stored somewhere, ideally not in the registry (as it should not be in the same place as the encrypted data). I'm not sure where to put this; maybe a file on the file system?
  * A key derivation scheme must be chosen or designed so we do not use the random bytes directly (i.e. acquiring these bytes will not directly compromise the encrypted data).
  * New UI elements must be added to ask the user's preference on this option; we don't want to do this without letting them know we're doing it, even if it's in their best interests.
  * This option must be backward compatible in that if the user chooses not to use a truly random key it should default to the current implementation.
  * If we're going this far, it makes sense to make this option available on-demand. Current users will need to re-key old user/machine keyed data anyway, so we might as well allow the user to re-key with a new random key any time they want.