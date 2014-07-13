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
    using CLAP.Validation;
    using Octokit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

	class App
	{
		[Help(Aliases = "?,h")]
		void Help(string help)
		{
			Console.WriteLine(help);
		}

		[Verb(Description = "Generates a status report from GitHub issues.", IsDefault = true)]
		void Generate(
            [DefaultProvider(typeof(OutDirProvider))]
            [Aliases("d")]
            string outDir,
            [DefaultProvider(typeof(OwnerDefaultProvider))]
            [Aliases("o")]
            string owner, 
            [DefaultProvider(typeof(RepoDefaultProvider))]
            [Aliases("r")]
            string repo, 
            [DefaultProvider(typeof(TokenProvider))]
            [Aliases("t")]
            string apiToken,
            [Description("Maximum age in days of issues to consider for the generation.")]
            [DefaultValue("180")]
            [Aliases("a")] 
            int maxAge)
		{
            var github = new GitHubClient(new ProductHeaderValue("OctoFlow"))
            {
                Credentials = new Credentials(apiToken)
            };

            var process = new ProcessRepository(
                github,
                new ProcessSettings(owner, repo) { MaxAge = TimeSpan.FromDays(maxAge) });

            process.Initialize();

            var type = ProcessType.QA;
			var flow = process.GetFlow(type);

            var generator = new FlowIssueGenerator(type, flow);

            var body = generator.Render();

            Console.WriteLine("Generating for {0}/{1}, with token {2}", owner, process, apiToken);
		}

		[Verb(Description = "Get the application version.", Aliases = "v")]
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
	}
}
