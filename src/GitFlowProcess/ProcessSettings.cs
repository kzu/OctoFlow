/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/
namespace GitFlowProcess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProcessSettings
    {
        public ProcessSettings(string repoOwner, string repoName)
        {
            this.MaxAge = TimeSpan.FromDays(180);
            this.Repository = new RepositorySettings(repoOwner, repoName);
        }

        /// <summary>
        /// Process issues that are at most the specified age. 
        /// Defaults to 180 days.
        /// </summary>
        public TimeSpan MaxAge { get; set; }

        public RepositorySettings Repository { get; private set; }
    }

    public class RepositorySettings
    {
        public RepositorySettings(string owner, string name)
        {
            this.Owner = owner;
            this.Name = name;
        }

        public string Owner { get; private set; }
        public string Name { get; private set; }
    }
}
