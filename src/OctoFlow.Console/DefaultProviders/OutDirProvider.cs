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
    using System.Text;
    using System.Threading.Tasks;

    public class OutDirProvider : DefaultProvider
    {
        public override object GetDefault(VerbExecutionContext context)
        {
            var gitRoot = GitRepo.Find(".");
            if (string.IsNullOrEmpty(gitRoot))
                return new DirectoryInfo(".").FullName;

            return gitRoot;
        }

        public override string Description
        {
            get { return "git repo root or current dir"; }
        }
    }
}
