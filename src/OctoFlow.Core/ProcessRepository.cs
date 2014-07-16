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
	using Octokit.Reactive;
	using System;
	using System.Collections.Generic;
	using System.Reactive.Disposables;
	using System.Threading;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Octokit.Internal;

	public class ProcessRepository : IProcessRepository
	{
		const string CheckSymbol = "\x2714"; //<- Equivalent to ✓
		static readonly Regex issueLink = new Regex(@"\#(?<number>\d+)", RegexOptions.Compiled);
		static readonly Regex releaseLabel = new Regex(@"v?(?<version>\d+.\d+(.\d+)?)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		IObservableGitHubClient github;

		CountdownEvent downloadCompleted = new CountdownEvent(3);
		CompositeDisposable subscriptions = new CompositeDisposable();

		public ProcessRepository(IGitHubClient github, ProcessSettings settings)
		{
			GitHub = github;
			AllIssues = new Dictionary<int, ProcessIssue>();
			AllLabels = new List<Label>();
			Settings = settings;

			this.github = new ObservableGitHubClient(github);
		}

		public ProcessRepository(IEnumerable<Issue> issues)
		{
			AllIssues = issues.ToDictionary(
				i => i.Number,
				i => new ProcessIssue(i));
			AllLabels = AllIssues.Values
				.SelectMany(i => i.Issue.Labels)
				.Distinct(new SelectorComparer<Label, string>(l => l.Name))
				.ToList();

			// Signal both the load of closed and open issues.
			downloadCompleted.Signal(2);

			SetIssueTypes();
			SetIssueParent();
		}

		public IGitHubClient GitHub { get; private set; }
		public IDictionary<int, ProcessIssue> AllIssues { get; private set; }
		public IList<Label> AllLabels { get; private set; }
		public ProcessSettings Settings { get; private set; }

		public void Initialize()
		{
			if (github == null)
				throw new InvalidOperationException();

			LoadAllIssues(ItemState.Open);
			LoadAllIssues(ItemState.Closed);
			LoadAllLabels();

			downloadCompleted.Wait();

			SetIssueTypes();
			SetIssueParent();
		}

		public IEnumerable<IGrouping<string, ProcessIssue>> GetFlow(ProcessType type)
		{
			switch (type)
			{
				case ProcessType.Dev:
					break;
				case ProcessType.QA:
					return GetQAFlow();
				case ProcessType.Doc:
					return GetDocFlow();
				default:
					break;
			}

			throw new NotSupportedException("Specified flow '" + type + "' is not supported.");
		}

		private void SetIssueTypes()
		{
			foreach (var issue in AllIssues.Values)
			{
				var labels = new HashSet<string>(issue.Issue.Labels.Select(l => l.Name.ToLowerInvariant()));

				// Determine issue type
				if (labels.Contains("story"))
					issue.Type = IssueType.Story;
				else if (labels.Contains("task"))
					issue.Type = IssueType.Task;
				else if (labels.Contains("bug"))
					issue.Type = IssueType.Bug;
			}
		}

		private void SetIssueParent()
		{
			foreach (var issue in AllIssues.Values.Where(p => p.Type != IssueType.Story))
			{
				// Parent bugs and tasks with their parent stories, if any.
				var linkMatches = issueLink.Matches(issue.Issue.Body);
				foreach (var linkMatch in linkMatches.OfType<Match>())
				{
					ProcessIssue linkedIssue;
					if (AllIssues.TryGetValue(int.Parse(linkMatch.Groups["number"].Value), out linkedIssue) &&
						linkedIssue.Type == IssueType.Story)
					{
						linkedIssue.Children.Add(issue);
					}
				}
			}
		}

		private void LoadAllIssues(ItemState state)
		{
			subscriptions.Add(github.Issue
				.GetForRepository(
					Settings.Repository.Owner,
					Settings.Repository.Name,
					new PagedRepositoryIssueRequest
					{
						PerPage = 100,
						Since = DateTimeOffset.Now.Subtract(Settings.MaxAge),
						State = state,
					})
				.Subscribe(
					issue => AllIssues.Add(issue.Number, new ProcessIssue(issue)),
					() => downloadCompleted.Signal()));
		}

		private void LoadAllLabels()
		{
			subscriptions.Add(github.Issue.Labels
				.GetForRepository(
					Settings.Repository.Owner,
					Settings.Repository.Name)
				.Subscribe(
					label => AllLabels.Add(label),
					() => downloadCompleted.Signal()));
		}

		private IEnumerable<IGrouping<string, ProcessIssue>> GetQAFlow()
		{
			// Establish the state of all issues WRT to this process flow.
			foreach (var issue in AllIssues.Values)
			{
				if (issue.Issue.Labels.Any(l => string.Equals(l.Name, "-QA", StringComparison.OrdinalIgnoreCase)) ||
					issue.Issue.State == ItemState.Open)
				{
					issue.State = ProcessState.Ignore;
				}
				else if (issue.Issue.Labels.Any(l => string.Equals(l.Name, CheckSymbol + "qa", StringComparison.OrdinalIgnoreCase)))
				{
					issue.State = ProcessState.Done;
				}
				else
				{
					if (issue.Issue.Assignee != null)
						issue.State = ProcessState.Doing;
					else
						issue.State = ProcessState.ToDo;
				}

                GroupFromReleaseLabel(issue);
			}

			var result = AllIssues.Values.Where(issue =>
					issue.Type == IssueType.Story &&
						// Keep stories that aren't ignored, that is, they are still open
					issue.State != ProcessState.Ignore
                        // Open stories aren't ever ready for the next flow, 
                        // neither any of its child issues. Otherwise, it's 
                        // just plain confusing.
                        //issue.Children.Any(child => child.State != ProcessState.Ignore)
					 )
					.ToList();

			// Next add all issues that don't have a parent story.
			result.AddRange(AllIssues.Values.Where(issue =>
				issue.State != ProcessState.Ignore &&
				issue.Parent == null));

            return result.OrderByDescending(p => p.Sort).GroupBy(p => p.Group);
		}

		private IEnumerable<IGrouping<string, ProcessIssue>> GetDocFlow()
		{
			// Establish the state of all issues WRT to this process flow.
			foreach (var issue in AllIssues.Values)
			{
				if (issue.Issue.State == ItemState.Open || 
					!issue.Issue.Labels.Any(l => 
						string.Equals(l.Name, "+Doc", StringComparison.OrdinalIgnoreCase) || 
						string.Equals(l.Name, CheckSymbol + "Doc", StringComparison.OrdinalIgnoreCase))
					)
				{
					issue.State = ProcessState.Ignore;
				}
				else if (issue.Issue.Labels.Any(l => string.Equals(l.Name, CheckSymbol + "Doc", StringComparison.OrdinalIgnoreCase)))
				{
					issue.State = ProcessState.Done;
				}
				else
				{
					issue.State = ProcessState.ToDo;
				}

                GroupFromReleaseLabel(issue);
			}

			var result = AllIssues.Values.Where(issue =>
					issue.Type == IssueType.Story &&
						// Keep stories that aren't ignored 
					issue.State != ProcessState.Ignore
                        // Open stories aren't ever ready for the next flow, 
                        // neither any of its child issues. Otherwise, it's 
                        // just plain confusing.
                        //issue.Children.Any(child => child.State != ProcessState.Ignore)
					 )
					.ToList();

			// Next add all issues that don't have a parent story.
			result.AddRange(AllIssues.Values.Where(issue =>
				issue.State != ProcessState.Ignore &&
				issue.Parent == null));

            return result.OrderByDescending(p => p.Sort).GroupBy(p => p.Group);
		}

        private void GroupFromReleaseLabel(ProcessIssue issue)
        {
            var match = issue.Issue.Labels
                .Select(l => releaseLabel.Match(l.Name))
                .FirstOrDefault(m => m.Success);

            if (match != null)
            {
                issue.Group = "Release " + match.Groups["version"].Value;
                issue.Sort = new Version(match.Groups["version"].Value);
            }
            else
            {
                issue.Group = "vNext";
                issue.Sort = new Version(int.MaxValue, int.MaxValue);
            }
        }

		class PagedRepositoryIssueRequest : RepositoryIssueRequest
		{
			public PagedRepositoryIssueRequest()
			{
				this.PerPage = 100;
			}

			[Parameter(Key = "per_page")]
			public int PerPage { get; set; }
		}

		class SelectorComparer<T, TResult> : IEqualityComparer<T>
		{
			private Func<T, TResult> selector;

			public SelectorComparer(Func<T, TResult> selector)
			{
				this.selector = selector;
			}

			public bool Equals(T x, T y)
			{
				return Object.Equals(selector(x), selector(y));
			}

			public int GetHashCode(T obj)
			{
				return selector(obj).GetHashCode();
			}
		}
	}
}
