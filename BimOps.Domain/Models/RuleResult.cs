using System;
using System.Collections.Generic;
using System.Text;

namespace BimOps.Domain.Models
{
    public class RuleResult
    {
        public string RuleId { get; set; }
        public string RuleName { get; set; }
        public RuleSeverity Severity { get; set; }
        public string Message { get; set; }
        public string ElementUniqueId { get; set; }
        public int? ElementId { get; set; }
        public string SuggestedFix { get; set; }
        public bool CanAutoFix { get; set; }
    }
}
