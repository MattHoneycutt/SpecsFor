using System;
using NUnit.Framework;
using Should.Core.Exceptions;
using SpecsFor.ShouldExtensions;

namespace SpecsFor.Tests.ShouldExtensions
{
	public class ShouldLooksLikeExtensionsSpecs : SpecsFor<ShouldLooksLikeExtensionsSpecs.TestObject>
	{
		public class TestObject
		{
			public Guid TestObjectId { get; set; }
			public int Awesomeness { get; set; }
			public string Name { get; set; }
			public TestObject Nested { get; set; }
			public TestObject[] NestedArray { get; set; }
		}

		protected override void InitializeClassUnderTest()
		{
			SUT = new TestObject { TestObjectId = Guid.NewGuid(), Name = "Test", Awesomeness = 11};
		}

		[Test]
		public void then_it_should_only_check_specified_properties()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Test",
				Awesomeness = 11
			}));
		}

		[Test]
		public void then_it_should_like_guids_too()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				TestObjectId = SUT.TestObjectId
			}));
		}

		[Test]
		public void then_it_should_fail_if_specified_properties_dont_match()
		{
			Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Tests"
			}));
		}

		[Test]
		public void then_it_should_work_with_nested_objects()
		{
			SUT.Nested = new TestObject
			{
				Name = "nested 1 test",
				Awesomeness = -10, //not going to specify in assertion
				Nested = new TestObject
				{
					Name = "ULTRA NEST COMBO KILL",
					Awesomeness = 69 //thanks, Bill & Ted, real mature.
				}
			};

			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Test",
				Awesomeness = 11,
				Nested = new TestObject
				{
					Name = "nested 1 test",
					Nested = new TestObject
					{
						Name = "ULTRA NEST COMBO KILL",
						Awesomeness = 69
					}
				}
			}));
		}

		[Test]
		public void then_it_should_fail_with_incorrectly_nested_objects()
		{
			SUT.Nested = new TestObject
			{
				Name = "nested 1 test",
				Awesomeness = -10, //not going to specify in assertion
				Nested = new TestObject
				{
					Name = "ULTRA NEST COMBO KILL",
					Awesomeness = 69 //thanks, Bill & Ted, real mature.
				}
			};

			Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Test",
				Awesomeness = 11,
				Nested = new TestObject
				{
					Name = "nested 1 test",
					Nested = new TestObject
					{
						Name = "ULTRA NEST COMBO KILL",
						Awesomeness = 70
					}
				}
			}));
		}

		[Test]
		public void then_it_should_work_with_ienumerables()
		{
			var list = new[]
				{
					new TestObject
						{
							Name = "one",
							Awesomeness = 1
						},
					new TestObject
						{
							Name = "two",
							Awesomeness = 2
						}
				};

			Assert.DoesNotThrow(() => list.ShouldLookLike(() => new[]
				{
					new TestObject
						{
							Name = "one",
							Awesomeness = 1
						},
					new TestObject
						{
							Name = "two",
							Awesomeness = 2
						}
				}));
		}

		[Test]
		public void then_it_should_fail_when_ienumerables_dont_match()
		{
			var list = new[]
				{
					new TestObject
						{
							Name = "one",
							Awesomeness = 1
						},
					new TestObject
						{
							Name = "two",
							Awesomeness = 2
						}
				};

			Assert.Throws<EqualException>(() => list.ShouldLookLike(() => new[]
				{
					new TestObject
						{
							Name = "one",
							Awesomeness = 1
						},
					new TestObject
						{
							Name = "two",
							Awesomeness = 3
						}
				}));
		}

		[Test]
		public void then_it_should_work_with_nested_ienumerables()
		{
			SUT.Nested = new TestObject
			{
				Name = "nested 1 test",
				Awesomeness = -10, //not going to specify in assertion
				NestedArray = new []
					{
						new TestObject
							{
								Name = "level 1 nested 1",
								Awesomeness = 11
							},
						new TestObject
							{
								Name = "level 1 nested 2",
								Awesomeness = -4, //not specified in assertion
								NestedArray = new []
									{
										new TestObject
											{
												Name = "lets get nested son"
											}
									}
							}
					},
				Nested = new TestObject
				{
					Name = "ULTRA NEST COMBO KILL",
					Awesomeness = 69, //thanks, Bill & Ted, real mature.
					NestedArray = new []
						{
							new TestObject
								{
									Name = "nested array 1",
									Awesomeness = -12 //not specified in assertion
								},
							new TestObject
								{
									Name = "nested array 2"
								}
						}
				}
			};

			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Test",
				Awesomeness = 11,
				Nested = new TestObject
				{
					Name = "nested 1 test",
					NestedArray = new []
						{
							new TestObject
								{
									Name = "level 1 nested 1",
									Awesomeness = 11
								},
							new TestObject
								{
									Name = "level 1 nested 2",
									NestedArray = new []
										{
											new TestObject
												{
													Name = "lets get nested son"
												}
										}
								}
						},
					Nested = new TestObject
					{
						Name = "ULTRA NEST COMBO KILL",
						Awesomeness = 69, //thanks, Bill & Ted, real mature.
						NestedArray = new []
							{
								new TestObject
									{
										Name = "nested array 1",
									},
								new TestObject
									{
										Name = "nested array 2"
									}
							}
					}
				}
			}));
		}

		[Test]
		public void then_it_should_fail_with_bad_nested_ienumerables()
		{
			SUT.Nested = new TestObject
			{
				Name = "nested 1 test",
				Awesomeness = -10, //not going to specify in assertion
				NestedArray = new[]
					{
						new TestObject
							{
								Name = "level 1 nested 1",
								Awesomeness = 11
							},
						new TestObject
							{
								Name = "level 1 nested 2",
								Awesomeness = -4, //not specified in assertion
								NestedArray = new []
									{
										new TestObject
											{
												Name = "lets get nested son"
											}
									}
							}
					},
				Nested = new TestObject
				{
					Name = "ULTRA NEST COMBO KILL",
					Awesomeness = 69, //thanks, Bill & Ted, real mature.
					NestedArray = new[]
						{
							new TestObject
								{
									Name = "nested array 1",
									Awesomeness = -12 //not specified in assertion
								},
							new TestObject
								{
									Name = "nested array 2"
								}
						}
				}
			};

			Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new TestObject
			{
				Name = "Test",
				Awesomeness = 11,
				Nested = new TestObject
				{
					Name = "nested 1 test",
					NestedArray = new[]
						{
							new TestObject
								{
									Name = "level 1 nested 1",
									Awesomeness = 11
								},
							new TestObject
								{
									Name = "level 1 nested 2",
									NestedArray = new []
										{
											new TestObject
												{
													Name = "lets get nested son OH NO THIS IS WRONG"
												}
										}
								}
						},
					Nested = new TestObject
					{
						Name = "ULTRA NEST COMBO KILL",
						Awesomeness = 69, //thanks, Bill & Ted, real mature.
						NestedArray = new[]
							{
								new TestObject
									{
										Name = "nested array 1",
									},
								new TestObject
									{
										Name = "nested array 2"
									}
							}
					}
				}
			}));
		}

		[Test]
		public void then_it_throws_a_good_error_if_you_pass_in_anything_but_a_member_init_expression()
		{
			var testObject = new TestObject {};

			Assert.Throws<InvalidOperationException>(() =>
			{
				SUT.ShouldLookLike(() => testObject);
			});
		}
	}
}