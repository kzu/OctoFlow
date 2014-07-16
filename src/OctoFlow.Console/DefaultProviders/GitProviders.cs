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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public abstract class GitDefaultProvider : DefaultProvider
    {
        static readonly Regex urlExpr = new Regex(@"[/:](?<owner>[^/]+)/(?<repository>[^/]+)\.git", RegexOptions.Compiled);

        public override object GetDefault(VerbExecutionContext context)
        {
            var gitDir = GitRepo.Find(".");
            if (gitDir == null)
                throw new InvalidOperationException("Could not locate a git repository in the current directory or any of its ancestors.");

            // Read git repo from current path, if it exists.
            try
            {
                var lines = File.ReadAllLines(Path.Combine(gitDir, ".git", "config"));
                var remotes = lines
                    .Select((line, i) => new { Line = line, Index = i })
                    // Only those lines that define a remote
                    .Where(line => line.Line.StartsWith("[remote "))
                    // Grab url from following line.
                    .Select(line => new { Remote = line.Line.Substring(9, line.Line.Length - 11), Url = lines[line.Index + 1].Trim() })
                    .ToList();

                var remote = remotes
                    .FirstOrDefault(x =>
                        x.Remote == "origin" ||
                        x.Remote == "upstream");

                if (remote == null)
                    remote = remotes.FirstOrDefault();

                if (remote != null)
                {
                    var match = urlExpr.Match(remote.Url);
                    if (match.Success)
                        return GetDefault(match.Groups["owner"].Value, match.Groups["repository"].Value);
                }
            }
            catch (IOException) { }

            return null;
        }

        public override string Description
        {
            get { return "from git remote 'origin', 'upstream' or first found"; }
        }

        protected abstract object GetDefault(string owner, string repository);
    }

    public class RepoDefaultProvider : GitDefaultProvider
    {
        protected override object GetDefault(string owner, string repository)
        {
            return repository;
        }
    }

    public class OwnerDefaultProvider : GitDefaultProvider
    {
        protected override object GetDefault(string owner, string repository)
        {
            return owner;
        }
    }
}
