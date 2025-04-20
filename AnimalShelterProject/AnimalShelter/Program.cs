namespace AnimalShelter;

using System.IO;
using System;
using Spectre.Console;
using Microsoft.VisualBasic;

class Program
{

    static void Main(string[] args)
    {
        UI ui = new UI();
        ui.LoginScreen();
    }

}