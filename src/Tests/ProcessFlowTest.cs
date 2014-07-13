namespace GitFlowProcess
{
    using GitFlowProcess;
    using Octokit;
    using System;
    using System.IO;
    using Xunit;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProcessFlowTest
    {
        static readonly Credentials credentials = new Credentials(File.ReadAllText(@"..\..\Token").Trim());

        IGitHubClient github;

        public ProcessFlowTest()
        {
            github = new GitHubClient(new ProductHeaderValue("kzu-client"))
            {
                Credentials = credentials
            };

            var repository = new ProcessRepository(
                github,
                new ProcessSettings("netfx", "LearnGit"));

            repository.Initialize();
            Repository = repository;
        }

        public IProcessRepository Repository { get; set; }

        [Fact]
        public async Task when_generating_flow_then_succeeds()
        {
            var type = ProcessType.QA;
			var flow = Repository.GetFlow(type);

            var generator = new FlowIssueGenerator(type, flow);

            var body = generator.Render();

            var issue = Repository.AllIssues.Values.FirstOrDefault(i => i.Issue.Title == "~" + type);
            if (issue != null)
            {
                var updated = await github.Issue.Update(
                    Repository.Settings.Repository.Owner,
                    Repository.Settings.Repository.Name,
                    issue.Issue.Number,
                    new IssueUpdate
                    {
                        Body = body
                    });

                Console.WriteLine("Updated issue #" + updated.Number);
            }
            else
            {
                var created = await github.Issue.Create(
                    Repository.Settings.Repository.Owner,
                    Repository.Settings.Repository.Name,
                    new NewIssue("~" + type)
                    {
                        Body = body
                    });

                Console.WriteLine("Created issue #" + created.Number);
            }

            Console.WriteLine(body);
        }
    }
}
