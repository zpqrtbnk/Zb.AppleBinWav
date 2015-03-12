Zb.AppleBinWav
==============
A Windows WPF application that can send binary data to an Apple ][+ cassette input.

#### Unfinished business

The first computer I've ever used was an [HP 85](http://oldcomputers.net/hp85.html) but the first one we've actually *owned* was an Apple ][ europlus (the european version of the ][+). I was 12 or 14 at that time, and I remember copying pages of binary code from magazines, deciphering the motherboard schematics, and learning the 6502 assembly.

There was one thing I could not get right: to get a simple game sprite to move across the screen properly. I *did* manage to paint a background, read keyboard and get a sprite to move around, but with a nasty flicker effect. I had managed to understand that it was due to video synchronization. I had learned about double-buffering and page-flipping. I *knew* I had to take VBLANK in account but could not figure out how. Remember, at that time there was no Google, and all you could do was guess, experiment, count CPU cycles, and read magazines...

And then we sold the Apple and replaced it with a PC.

#### 6502 Reloaded

Almost by accident, I have recently been able to recover two original Apple ][ europlus. (S/N IA2S2-504523 from 1980 and S/N IA2S2-677648 from 1982). Thanks to Google, I managed to find [this 1982 article](http://rich12345.tripod.com/aiivideo/softalk.html) by Bob Bishod, and then a copy of [Enhancing Your Apple II (vol2)](http://apple2online.com/web_documents/Enhancing%20Your%20Apple%20IIe%20Vol%202%20KB.pdf) by Don Lancaster, where Don describes the VAPORLOCK mechanism.

These were the missing parts, all I needed to go back to my 30 year-old unfinished business. The VBLANK detection mechanism had to be slightly adapted for europlus machines due to 50Hz vs. 60Hz video signal synchronization, but once you know what you are looking for, it gets easy.

Oh, and I still remembered that $A9 was the hexadecimal value for the 6502 immediate accumulator load LDA instruction...

An (undisclosed) number of hours + a blown PSU capacitor later, I had what I wanted: a game sprite moving smoothly accross the screen without the trace of a flicker. The 14yo in me is happy! You can find the source code for this beauty in the *asm* directory: it's probably ugly, but it works (on real, non-cloned, ][+ or europlus, that is - probably does not run on //e and above nor on clone - though it runs on the [AppleWin](https://github.com/AppleWin/AppleWin) emulator).

#### AppleBinWav

I began by manually assembling my code, like it was 1980 again, but quickly find it easier to write the code on a PC and cross-assemble using the [as65 assembler](http://www.kingswood-consulting.co.uk/assemblers/). Depending on the options, it can generate either a raw binary that you can load in AppleWin, or a nice binary file using the [Intel-HEX](http://en.wikipedia.org/wiki/Intel_HEX) file format.

Problem is, that binary file needs to be transfered to the Apple. Which has no disk drive, no serial port, no ethernet, no nothing. Except for cassette IN and OUT ports. And so AppleBinWav was born. It reads an Intel-HEX binary file, generates a WAV signal with code highly inspired from [ADT Pro](http://adtpro.sourceforge.net/index.html), ported to C# and slightly adapted, and then plays the file in the ears of the Apple. Problem solved.

Funny enough, the specs for the cassette signal are still on Apple's website, see [The Apple II Cassette Interface (1 of 2)](http://support.apple.com/kb/TA40730) and [The Apple II Cassette Interface (2 of 2)](http://support.apple.com/kb/TA40737). Interesting infos are also available [here](http://www.pagetable.com/?p=32).

#### Usage

Plug an audio cable from the PC audio-OUT to the Apple cassette IN port.

Click *Source* to open a data file. Once the data has been loaded successfully, AppleBinWav will display the command to issue on the Apple to initiate the transfer, eg "300.3A8R". You will need to enter the monitor (CALL -151) and then initiate the transfer, then click *Play* in AppleBinWav to send data to the cassette IN port. If all goes well, the monitor should return with a beep and you should be able to execute the transfered code:

```
]CALL -151
*300.3A8R
*300G
```

*DISCLAIMER* This program was created for my own usage. There's already far too much WPF chrome in there (could not resist styling the buttons...) but many thing can go wrong. If the source file is not in the right format, if something goes wrong, if... the program will probably just crash, but anything really can happen.

If you feel like improving it, just fork and PR!

Enjoy!
