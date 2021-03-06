﻿using System;
using TwentyQuestions.Core;

namespace TwentyQuestions
{
    public class ConsoleService : IConsoleService
    {
        public void WriteLine(string value) => Console.WriteLine(value);

        public string ReadLine() => Console.ReadLine();
    }
}
