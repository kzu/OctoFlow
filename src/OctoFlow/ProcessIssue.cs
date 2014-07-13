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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    public class ProcessIssue
    {
        public ProcessIssue(Issue issue)
        {
            this.Issue = issue;

            var collection = new ObservableCollection<ProcessIssue>();
            collection.CollectionChanged += (sender, args) =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        args.NewItems.OfType<ProcessIssue>()
                            .ToList()
                            .ForEach(i => i.Parent = this);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        args.NewItems.OfType<ProcessIssue>()
                            .ToList()
                            .ForEach(i => i.Parent = null);
                        break;
                    default:
                        break;
                }
            };

            Children = collection;
        }

        public string GroupLabel { get; set; }
        public Issue Issue { get; private set; }
        public IssueType Type { get; set; }
		public ProcessState State { get; set; }
        public ProcessIssue Parent { get; private set; }
        public IList<ProcessIssue> Children { get; private set; }

        public override string ToString()
        {
            return Type.ToString() + " #" + Issue.Number + ": " + Issue.Title;
        }
    }
}