Zb.AppleBinWav
==============
A Windows WPF application that can send binary data to an Apple ][+ cassette input.

The binary data must be in the [Intel-HEX](http://en.wikipedia.org/wiki/Intel_HEX) file format. It is possible to produce such a file by assembling ASM source with an assembler such as [as65](http://www.kingswood-consulting.co.uk/assemblers/) using the appropriate command-line option.

The WAV-generation code is highly inspired from [ADT Pro](http://adtpro.sourceforge.net/index.html) though it has been ported to C# and slightly adapted.

Once the data has been loaded successfully, the application will display the command to issue on the Apple ][+ to initiate the transfer, eg "300.3A8R". You will need to enter the monitor (CALL -151) and then type.

```
]CALL -151
*300.3A8R
*300G
```

Enjoy!

