using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitFlowProcess
{
    partial class FlowIssueGenerator
    {
        public FlowIssueGenerator(ProcessType type, IEnumerable<IGrouping<string, ProcessIssue>> issues)
        {
            Type = type;
            Issues = issues;
        }

        public string Render()
        {
            return TransformText();
        }
    }
}
