using BimOps.Domain.Models;

namespace BimOps.Domain.Rules
{
    public interface IModelRule
    {
        string Id { get; }
        string Name { get; }
        RuleSeverity Severity { get; }

        IEnumerable<RuleResult> Evaluate(ModelSnapshot snapshot);
    }
}
