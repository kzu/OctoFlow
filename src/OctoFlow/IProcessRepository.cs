/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/
namespace OctoFlow
{
    using Octokit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public interface IProcessRepository
    {
        IDictionary<int, ProcessIssue> AllIssues { get; }
        IList<Label> AllLabels { get; }
        IGitHubClient GitHub { get; }
        ProcessSettings Settings { get; }

		IEnumerable<IGrouping<string, ProcessIssue>> GetFlow(ProcessType type);
    }
}
