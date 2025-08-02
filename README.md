![C#](https://img.shields.io/badge/platforms-Windows-blue?style=flat-square)
![C#](https://img.shields.io/badge/platforms-Linux-blue?style=flat-square)
![C#](https://img.shields.io/badge/platforms-MacOS-blue?style=flat-square)
![C#](https://img.shields.io/badge/Programming-C%23-green?logo=csharp)
![C#](https://img.shields.io/badge/Framework-.NET8-green)
![C#](https://img.shields.io/badge/Licence-MIT-yellow)

# FCryptifier
FCryptifier is an easy-to-use console program that encrypts and decrypts files using the AES algorithm.
A graphical Windows application is also available for ease of use.

Created by [Gerald Weinberger](g.weinberger@outlook.com) with the aim of providing a free, open-source solution for encrypting and decrypting files. Because data security should be accessible and free for everyone.

**FCryptifier pillars are:**
- Ease of use
- Multiplatform
- Open-Source and ready for contribution
- Modern C# code

## Contributing

[CODE_OF_CONDUCT](CODE_OF_CONDUCT.md)<br>
Check out our [issues](https://github.com/gweinberger/FCryptifier/issues) and [pull requests](https://github.com/gweinberger/FCryptifier/pulls) to learn more about what's coming next!
<br>
Last changes see [Changelog](CHANGELOG.md)

## Downloads

Clone the repository and compile it yourself, or choose a download that matches your platform.
<br>
Current releases: [Downloads](https://github.com/gweinberger/FCryptifier/releases)

## Usage

You can encrypt any filetype. Encrypted files are saved with the extension `.aes`. This tells the program whether it needs to encrypt or decrypt the files.<br>
An individual salt is generated for each file so that repeated encryption of the same file produces a different result.

### Windows Desktop-App
Start `FCryptifierWinUI.exe` and select one or more files. You can also use drag and drop.

### Terminal

```bash
fcryptifier -e|-d -f inputfile -p password | -pf passwordfile

# Example: encrypt file.txt with password 1234
fcryptifier -e -f file.txt -p 1234

# Example: decrypt secure.txt.aes with passwordfile pass.dat
fcryptifier -d -f secure.txt.aes -pf pass.dat
```
