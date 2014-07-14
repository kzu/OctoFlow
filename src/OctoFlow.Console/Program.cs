/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/

namespace OctoFlow
{
	using CLAP;
    using OctoFlow.Diagnostics;
    using System;
    using System.Diagnostics;
    using System.Linq;

    class Program
	{
		static void Main(string[] args)
		{
            Tracer.Initialize(new TracerManager());

            Parser.Run(args, new App());

#if DEBUG
            Console.ReadLine();
#endif
		}
	}
}
