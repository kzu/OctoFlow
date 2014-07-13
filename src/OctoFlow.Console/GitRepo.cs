/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/

namespace OctoFlow
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class GitRepo
    {
        public static string Find(string baseDir)
        {
            return AncestorOrSelf(baseDir)
                .Where(d => d.EnumerateDirectories(".git").Any())
                .Select(d => d.FullName)
                .FirstOrDefault();
        }

        private static IEnumerable<DirectoryInfo> AncestorOrSelf(string baseDir)
        {
            var directory = new DirectoryInfo(".");
            while (directory != null)
            {
                yield return directory;
                directory = directory.Parent;
            }
        }
    }
}
