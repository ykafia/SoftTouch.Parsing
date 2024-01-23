// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using SoftTouch.Parsing.SDSL;

Console.WriteLine("Hello, World!");

Console.WriteLine($"Size of CharTerminal is {Marshal.SizeOf<CharTerminal>()}");
Console.WriteLine($"Size of DigitTerminal is {Marshal.SizeOf<DigitTerminal>()}");
