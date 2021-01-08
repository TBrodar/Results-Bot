![Screen shot](https://github.com/TBrodar/Results-Bot/tree/master/ResultsBot/ResultsBot.png)

Purpose of ResultsBot software is to simplfy making of automatization software in python
and running that code. Example: Controling temperature controler/arduino via USB or some
measurement device via GPIB or using another program by sending mouse/keyboard events. 

Requirements:
- 4.0 Net framework is required
- visual c++ 2008 redistributable for optical character recognition (OCR) (if missing, exception will be thrown when OCR is used)
- NI488.2 GPIB drivers for GPIB communication (if missing, exception will be thrown when GPIB is used)
- NI VISA drivers for VISA communication (if missing, exception will be thrown when VISA is used)

Useage:
- Write code in IronPython code editor
- By using set up menu item fill python dictionary, which describes GUI object (like button),
  with object screen position info. [x,y] values are used in click events
  and image capture.
- Mouse, Keyboard, App, Screen, USB, OCR, GPIB and VISA are avaliable globaly and provide
  usefull functions implemened in C#. You can put additional python modules in Lib/site-packages directory
  for additional functions not allready avaliable by default in iron python.
- Use other functions implemented in python for other stuff, such as time.sleep for adding delays, threading for multitasking etc...


Licences:

Python code editor is made by Avalon Edit http://avalonedit.net/ (MIT licence)
Keyboard and mouse events are simulated by Mouse and Keyboard C# libary
https://www.codeproject.com/Articles/28064/Global-Mouse-and-Keyboard-Library (The Code Project Open License (CPOL) 1.02)
Optical character recognition uses Tesseract 3.0.2.0 (Apache License, Version 2.0)

Licence for C# code in this project which uses previusly mentioned code:

This code is provided by open source licence

MIT License

Copyright (c) 2018 Tomislav Brodar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.