namespace GitFlowProcess
{
    using GitFlowProcess;
    using Octokit;
    using System.IO;
    using Xunit;

    public abstract class ProcessFlowTest
    {
        static readonly Credentials credentials = new Credentials(File.ReadAllText(@"..\..\Token").Trim());

        protected ProcessFlowTest()
        {
            var repository = new ProcessRepository(
                new GitHubClient(new ProductHeaderValue("kzu-client"))
                {
                    Credentials = credentials
                }, 
                new ProcessSettings("kzu", "GitFlowProcess"));

            repository.Initialize();
            Repository = repository;
        }

        public IProcessRepository Repository { get; set; }
    }
}
