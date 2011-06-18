#Project Description
SpecsFor is another Behavior-Driven Development framework that focuses on ease of use for *developers* by minimizing testing friction.

#Main Features

SpecsFor is a Behavior-Driven Development style framework that puts developer productivity ahead of all other goals.  The current release features:

* AutoMocking - Easily configure and verify behavior.
* ReSharper Live Templates - Quickly create specs with only a few keystrokes.
* Clean Separation of Test State - Encapsulate test setup and reuse it across as many specs as you like.
Run The Same Specs Multiple Times With Different Contexts - SpecsFor allows you to assert the same things are true given any number of contexts.
* Mix And Match Contexts - Context can be combined and extended to support complex test setup without code duplication or excess noise in your specs.
* Declarative Context - Context can be established in many ways, including by simply marking your spec class with a special attribute.
* Works With Any NUnit Test Runner - No add-ins are needed, SpecsFor is fully compatible with all popular test runners including TestDriven.NET, Resharper, and TeamCity. 

#Examples

```csharp
[Given(typeof(TheCarIsNotRunning), typeof(TheCarIsParked))]
[Given(typeof(TheCarIsNotRunning))]
public class when_the_key_is_turned : SpecsFor<Car>
{
    public when_the_key_is_turned(Type[] context) : base(context){}

    protected override void When()
    {
        SUT.TurnKey();
    }

    [Test]
    public void then_it_starts_the_engine()
    {
        GetMockFor<IEngine>()
            .Verify(e => e.Start());
    }
}
public class when_the_key_is_turned_alternate_style : SpecsFor<Car>
{
    protected override void Given()
    {
        Given<TheCarIsNotRunning>();
        Given<TheCarIsParked>();

        base.Given();
    }

    protected override void When()
    {
        SUT.TurnKey();
    }

    [Test]
    public void then_it_starts_the_engine()
    {
        GetMockFor<IEngine>()
            .Verify(e => e.Start());
    }
}
```

#SpecsFor In Action

Check out the current version in action: http://www.youtube.com/watch?v=MVwguBsR6eA

See an earlier prototype of SpecsFor in action: http://www.youtube.com/view_play_list?p=982492E9FAE3F64A

Read more about SpecsFor at http://trycatchfail.com/blog