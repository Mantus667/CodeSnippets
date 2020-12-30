# Non Async Sequential Rule Engine

This demo engine has two configurations: hits the first failed rule then short-circuits; and runs all rules returns a dictionary contains any rules that failed.

## Sample implementation

### Context

```CSharp
public class CatRuleCtx
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public List<Owner> Owners { get; set; }
}
```

### Rules

```CSharp
public class MustHaveAtLeastOneCurrentOwnerRule : Rule<CatRuleCtx>
{
    public MustHaveAtLeastOneCurrentOwnerRule(int priority)
    {
        Priority = priority;
    }

    public override bool IsValid(CatRuleCtx ctx)
    {
        var currentOwners = ctx.Owners.FirstOrDefault(
            x => x.IsCurrent
        );

        return currentOwners != null;
    }
}

public class MustLoveCatRule : Rule<CatRuleCtx>
{
    public MustLoveCatRule(int priority)
    {
        Priority = priority;
    }

    public override bool IsValid(CatRuleCtx ctx)
    {
        var areTheyAllLoveCats = ctx.Owners
            .Where(x => x.IsCurrent)
            .All(
                x => x.Attitude == "has a cats loving heart"
            );

        return areTheyAllLoveCats;
    }
}

public class CurrentOwnersMustOver16YearsOld : Rule<CatRuleCtx>
{
    public CurrentOwnersMustOver16YearsOld(int priority)
    {
        Priority = priority;
    }

    public override bool IsValid(CatRuleCtx ctx)
    {
        var allOver16 = ctx.Owners
            .Where(x => x.IsCurrent)
            .All(
                x => Moment.IsOver(16, x.Dob)
            );

        return allOver16;
    }
}
```

### Engine

```CSharp
public class CatRules : RuleEngine<CatRuleCtx>
{
    public CatRules(CatRuleCtx ctx)
    {
        Ctx = ctx;

        Rules = new List<Rule<CatRuleCtx>> {
            new MustHaveAnOwnerRule(1),
            new MustLoveCatRule(3),
            new CurrentOwnersMustOver16YearsOld(2),
        };
    }
}
```

With this approach, each business requirement is a single Rule class we won’t have trouble to tell if we have implemented all the required rules. And as business requirement changes, a rule can be changed or removed without breaking other rules. New rules can be added in at will with confidence old rules will still work as it is. It demonstrates the effect of Open and Close principle.