/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/
namespace GitFlowProcess
{
	using CLAP;
	using CLAP.Validation;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading.Tasks;

	class App
	{
		[Help]
		void Help(string help)
		{
			Console.WriteLine(help);
		}

		[Verb(Description = "Get the application version.")]
		void Version()
		{
			var asm = this.GetType().Assembly;

			Console.Write(asm.GetCustomAttribute<AssemblyTitleAttribute>().Title);
			Console.Write(" version ");
			Console.WriteLine(asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);

			//[Microsoft .NET Framework, version 4.0.30319.34014]
			Console.Write("[Microsoft .NET Framework, version ");
			Console.Write(Environment.Version);
			Console.WriteLine("]");

			Console.WriteLine(asm.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright);
		}

		[Verb(Description = "Generates a status report from GitHub issues matching the given parameters.", IsDefault = true)]
		void Generate()
		{
		}

	}
}
