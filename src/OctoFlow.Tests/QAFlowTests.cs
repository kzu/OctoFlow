namespace OctoFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Octokit;
    using Xunit;
    using Newtonsoft.Json;
    using System.IO;
    using Moq;
    using Newtonsoft.Json.Linq;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    public class QAFlowTests
    {
        [Fact]
        public void when_given_issues_then_should_cataloge_process_status()
        {
            var issues = JsonConvert.DeserializeObject<List<Issue>>(File.ReadAllText(@"..\..\json\qa.json"));
            var process = new ProcessRepository(issues);

            var flow = process.GetFlow(ProcessType.QA);

            var generator = new FlowIssueGenerator(ProcessType.QA, flow);

            var body = generator.Render();

            Console.WriteLine(body);

            //var obj = JObject.Parse(body);

            //Console.WriteLine(obj.ToString(Formatting.Indented));
        }

        [Fact]
        public void when_action_then_assert()
        {
            var lines = File.ReadAllLines(@".\..\..\..\..\.git\config");
            //[remote "origin"]
            //    url = git@github.com:kzu/GitFlowProcess.git
            var remotes = lines
                .Select((line, i) => new { Line = line, Index = i })
                .Where(line => line.Line.StartsWith("[remote "))
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
                {
                    Console.WriteLine("owner: {0}, repo: {1}", match.Groups["owner"].Value, match.Groups["repository"].Value);
                }
            }

        }

        static readonly Regex urlExpr = new Regex(@"[/:](?<owner>[^/]+)/(?<repository>[^/]+)\.git", RegexOptions.Compiled);

        [Fact]
        public void when_action_then_assert2()
        {
            Console.WriteLine(TimeSpan.FromDays(180));
        }
    }
}
