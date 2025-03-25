using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Core;
using SpecsFor.Lamar.Tests.TestObjects;

namespace SpecsFor.Lamar.Tests;

public class SpecsForSpecs
{
    public class when_running_specs_with_no_given_and_no_when : SpecsFor<object>
    {
        [Test]
        public void then_the_SUT_is_still_initialized()
        {
            SUT.ShouldNotBeNull();
        }
    }

    public class when_running_specs_with_multiple_thens : SpecsFor<object>
    {
        private static int _whenCount;
        private static int _givenCount;

        protected override void Given()
        {
            _givenCount++;
        }

        protected override void When()
        {
            base.When();
            _whenCount++;
        }

        [Test]
        public void then_the_when_should_only_be_executed_once()
        {
            _whenCount.ShouldEqual(1);
        }

        [Test]
        public void then_the_when_should_still_only_be_executed_once()
        {
            _whenCount.ShouldEqual(1);
        }

        [Test]
        public void then_the_given_should_only_be_executed_once()
        {
            _givenCount.ShouldEqual(1);
        }

        [Test]
        public void then_the_given_should_still_only_be_executed_once()
        {
            _givenCount.ShouldEqual(1);
        }
    }

    public class when_tearing_down_and_the_system_under_test_is_disposable : SpecsFor<object>
    {
        private Mock<IDisposable> _sutMock;

        protected override void InitializeClassUnderTest()
        {
            _sutMock = new Mock<IDisposable>();
            SUT = _sutMock.Object;
        }

        protected override void When()
        {
            TearDown();
        }

        [Test]
        public void then_it_should_call_Dispose_on_the_SUT()
        {
            _sutMock.Verify(d => d.Dispose());
        }
    }

    public class when_using_mocked_deps : SpecsFor<MyTestSut>
    {
        private int _result;

        protected override void Given()
        {
            GetMockFor<IFoo>().Setup(x => x.Bar(It.IsAny<int>())).Returns(10);
        }

        protected override void When()
        {
            _result = SUT.TimesTwo(1);
        }

        [Test]
        public void then_it_uses_the_setup()
        {
            _result.ShouldEqual(20);
        }
    }

    public class when_using_concrete_deps : SpecsFor<MyTestSut>
    {
        private int _result;

        public override void ConfigureContainer(Container container)
        {
            container.Configure(x => x.AddTransient<IFoo, Foo>());
        }

        protected override void When()
        {
            // Bar = x * 3, then * 2, so result should should be 6
            _result = SUT.TimesTwo(1);
        }

        [Test]
        public void then_it_uses_the_conrete_class()
        {
            _result.ShouldEqual(6);
        }
    }

    public class when_using_parameterized_contexts : SpecsFor<object>
    {
        public class NestedContext : IContext<object>
        {
            private readonly string _name;

            public NestedContext(string name)
            {
                _name = name;
            }

            public void Initialize(ISpecs<object> state)
            {
                state.GetMockFor<TextWriter>().Object.Write(_name);
            }
        }

        protected override void Given()
        {
            Given(new NestedContext("Test1"));
            Given(new NestedContext("Test2"));
        }

        [Test]
        public void then_it_calls_both_contexts()
        {
            GetMockFor<TextWriter>()
                .Verify(w => w.Write("Test1"));

            GetMockFor<TextWriter>()
                .Verify(w => w.Write("Test2"));
        }
    }

    public class when_using_the_before_and_after_each_test_method : SpecsFor<object>
    {
        private int _beforeEachCallCount = 0;
        private int _nextExpectedBeforeEachCallCount = 1;

        private int _afterEachCallCount = 0;
        private int _nextExpectedAfterEachCallCount = 0;

        protected override void BeforeEachTest()
        {
            _beforeEachCallCount++;
        }

        [Test]
        public void then_the_values_increase_each_time_1()
        {
            _beforeEachCallCount.ShouldEqual(_nextExpectedBeforeEachCallCount);
            _nextExpectedBeforeEachCallCount++;

            _afterEachCallCount.ShouldEqual(_nextExpectedAfterEachCallCount);
            _nextExpectedAfterEachCallCount++;
        }

        [Test]
        public void then_the_values_increase_each_time_2()
        {
            _beforeEachCallCount.ShouldEqual(_nextExpectedBeforeEachCallCount);
            _nextExpectedBeforeEachCallCount++;

            _afterEachCallCount.ShouldEqual(_nextExpectedAfterEachCallCount);
            _nextExpectedAfterEachCallCount++;
        }

        protected override void AfterEachTest()
        {
            _afterEachCallCount++;
        }
    }
}