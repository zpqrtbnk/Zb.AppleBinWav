Zb.AppleBinWav
==============
A Windows WPF application that can send binary data to an Apple ][+ cassette input.

#### About

The binary data must be in the [Intel-HEX](http://en.wikipedia.org/wiki/Intel_HEX) file format. It is possible to produce such a file by assembling ASM source with an assembler such as [as65](http://www.kingswood-consulting.co.uk/assemblers/) using the appropriate command-line option.

The WAV-generation code is highly inspired from [ADT Pro](http://adtpro.sourceforge.net/index.html) though it has been ported to C# and slightly adapted. Funny enough, the specs for the audio are still on Apple's website, see [The Apple II Cassette Interface (1 of 2)](http://support.apple.com/kb/TA40730) and [The Apple II Cassette Interface (2 of 2)](http://support.apple.com/kb/TA40737). Interesting infos are also available [here](http://www.pagetable.com/?p=32).

Click *Source* to open a data file. Once the data has been loaded successfully, the application will display the command to issue on the Apple ][+ to initiate the transfer, eg "300.3A8R". You will need to enter the monitor (CALL -151) and then initiate the transfer, then click *Play* in the application to send data to the cassette input. If all goes well, the monitor should return with a beep and you should be able to execute the transfered code:

```
]CALL -151
*300.3A8R
*300G
```

#### But... why?

The first computer I've ever used was an [HP 85](http://oldcomputers.net/hp85.html) but the first one we've actually *owned* was an Apple ][ europlus (the european version of the ][+). I was 12 or 14 at that time, and I remember copying pages of binary code from magazines, learning the 6502 assembly... and trying to get a simple image to move across the screen.

The image *would* move all right, but there would be a nasty flicker effect. At that time I managed to understand that it was due to video synchronization. I learned about double-buffering and page-flipping. I *knew* I had to take VBLANK in account but could not figure out how. Remember, there was no Google, and all you could do was try to guess, experiment, and read magazines... And then we sold the Apple and replaced it with a PC.

I have recently been able to recover two machines (S/N IA2S2-504523 label "8046" from 1980 and S/N IA2S2-677648 label "8222" from 1982). Thanks to Google I managed to find [this 1982 article](http://rich12345.tripod.com/aiivideo/softalk.html) by Bob Bishod, and then a copy of *Enhancing Your Apple II* by Don Lancaster, where he describes the VAPORLOCK mechanism. These were the missing parts: although the VBLANK detection mechanism is slightly different on europlus machines due to 50Hz vs 60Hz video signal synchronization, I decided to get back to my 30 year-old unfinished business.

Because I was tired of copying my experimental code from my PC to the Apple manually, I wrote Zb.AppleBinWav to I could transfer the binaries over the cassette port.

And... I now have a smooth image moving accross the screen. Done. Source code in the *asm* directory: it's probably ugly, but it works.
