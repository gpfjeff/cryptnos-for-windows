# Project Status #

| **Current Release** | 1.3.3.63 |
|:--------------------|:---------|
| **Date of Release** | March 13, 2013 |
| **Next Release Milestone** | 1.3.4    |
| **Date of Next Release** | Unknown |
| **Status of Development** | Sporadic |

# Download Cryptnos #

The following links point to the latest version of the Cryptnos download files hosted on Google Drive. The GnuPG signature and SHA-1 hash for each file can also be found below. Jeff's current GnuPG signature can be found [here](https://www.gpf-comics.com/gnupg.php).

| **Download Type** | **GnuPG Signature** | **SHA-1 Hash** | **Size**|
|:------------------|:--------------------|:---------------|:--------|
| [Windows Installer](https://drive.google.com/uc?export=download&id=0B55ltyq5FildRm41RFpDWlVpSms) | [Signature](https://drive.google.com/uc?export=download&id=0B55ltyq5FildX0hTY09ia1JzN0k) | ed73f27d71d0e8d398bfa5a1018413adf56a11fd | 889kb |
| [Source Archive](https://drive.google.com/uc?export=download&id=0B55ltyq5FildcTZhdWEyNkxiVE0) | [Signature](https://drive.google.com/uc?export=download&id=0B55ltyq5FildalZWV3NVdUxRYWM) | 429bf43247f017ec4567854fdca664729f3a5f34 | 805kb |

# About Cryptnos #

Cryptnos is a multi-platform, Open Source application for generating strong, pseudo-random passwords using cryptographic hashes. It combines a unique "site token" such as a website domain name with a master password and runs this data through a cryptographic hash algorithm to produce a password that is unique, lengthy, seemingly random yet completely repeatable. Unlike similar products, however, it is exceedingly flexible. It is not a browser plugin, so it can be used with other applications outside the Web. It provides unparalleled versatility by letting you specify the cryptographic hash to use, how many iterations of the hash to perform, what characters to include, and how long the final password should be. Best of all, it is exceedingly secure. Your master and generated passwords are _**NEVER**_ stored, and the parameters to recreate your passwords are stored in an encrypted form.

This project relates to the Microsoft Windows (.NET 2.0) version. For the Google Android version, please visit its [GitHub project page.](https://github.com/gpfjeff/cryptnos-for-android) Please visit the [official Cryptnos site](http://www.cryptnos.com/) for [FAQs](http://www.cryptnos.com/faq/), [news](http://www.cryptnos.com/news/windows/), tips, and additional information.

Cryptnos makes use of the [Legion of the Bouncy Castle C# Crypto API](http://www.bouncycastle.org/csharp/), which is released under the [MIT X11 License](http://opensource.org/licenses/mit-license.php). The source for the Bouncy Castle code can be obtained from their site. All original code for Cryptnos is released under the [GNU General Public License, version 2](http://www.gnu.org/licenses/old-licenses/gpl-2.0.html).

Crpytnos also uses the [GPFUpdateChecker](https://github.com/gpfjeff/gpfupdatechecker) library to check for application updates.

You can also learn more about this repository via the [project wiki](https://github.com/gpfjeff/cryptnos-for-windows/tree/wiki).
