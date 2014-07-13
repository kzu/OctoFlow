namespace GitFlowProcess
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
	}
}
