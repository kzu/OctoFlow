namespace OctoFlow
{
	using Octokit;
	using Octokit.Internal;
	using System;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using Xunit;

    public class OctoTests
    {
        static readonly Credentials credentials = new Credentials(File.ReadAllText(@"..\..\Token").Trim());

        [Fact]
        public void when_unicode_then_succeeds()
        {
            Assert.False(char.IsLetterOrDigit('✓'));
            Assert.True(char.IsWhiteSpace(' '));

            Assert.True(char.IsPunctuation('#'));
            Assert.True(char.IsSymbol('✓'));
            Assert.True(char.IsPunctuation('-'));
            Assert.True(char.IsSymbol('+'));
        }

        [Fact]
        public void when_getting_rate_limit_then_succeeds()
        {
            var github = new GitHubClient(new ProductHeaderValue("kzu-client"))
            {
                Credentials = credentials
            };

            Console.WriteLine(github
                .Connection.GetHtml(new Uri(github.BaseAddress, "rate_limit")).Result.Body);
        }

        [Fact]
        public void when_retrieving_issues_then_can_filter_only_stories()
        {
            var github = new Octokit.Reactive.ObservableGitHubClient(
                new ProductHeaderValue("kzu-client"), new InMemoryCredentialStore(credentials));

            var issues = github.Issue.GetForRepository("xamarin", "XamarinVS", new RepositoryIssueRequest
            {
                Labels = { "Story" }, 
                State = ItemState.Open,
                Since = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(30)), 
            });

            var done = false;
            var subscription = issues.Subscribe(
                i => 
                {
                    Console.WriteLine(i.Title);
                },
                () => done = true);

            while (!done)
            {
                Thread.Sleep(100);
            }
        }

        [Fact]
        public async Task when_retrieving_unassigned_issue_then_assignee_is_null()
        {
            var github = new GitHubClient(
                new ProductHeaderValue("kzu-client"), new InMemoryCredentialStore(credentials));

			var issue = await github.Issue.Create("kzu", "sandbox", new NewIssue("Unassigned"));

			Assert.Null(issue.Assignee);
        }

	}
}
