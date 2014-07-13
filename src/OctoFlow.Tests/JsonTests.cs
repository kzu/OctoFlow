namespace OctoFlow
{
	using Newtonsoft.Json;
	using Octokit;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Xunit;

	public class JsonTests
	{
		[Fact]
		public void when_deserializing_qa_then_succeeds()
		{
			var issues = JsonConvert.DeserializeObject<List<Issue>>(File.ReadAllText(@"..\..\json\qa.json"));

			Assert.NotNull(issues);
		}
	}
}
