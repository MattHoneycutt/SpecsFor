using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Should.Core.Exceptions;
using SpecsFor.Core.ShouldExtensions;

namespace SpecsFor.Autofac.Tests.ShouldExtensions
{
	public class ShouldLookLikeExtensionsSpecs : SpecsFor<ShouldLookLikeExtensionsSpecs.TestObject>
	{
		public class TestObject
		{
			public Guid TestObjectId { get; set; }
			public int Awesomeness { get; set; }
			public string Name { get; set; }
			public TestObject Nested { get; set; }
			public TestObject[] NestedArray { get; set; }
			public DateTime? OptionalDate { get; set; }
		}

		protected override void InitializeClassUnderTest()
		{
			SUT = new TestObject
			{
				TestObjectId = Guid.NewGuid(), 
				Name = "Test", 
				Awesomeness = 11,
				OptionalDate = DateTime.Today
			};
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
		public void then_it_should_pass_when_used_with_matcher_that_matches_anything()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				TestObjectId = Any.ValueOf<Guid>(),
				Name = Any.ValueOf<string>()
			}));
		}

		[Test]
		public void then_it_should_pass_when_used_with_matcher_that_matches_any_non_null_or_default()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				TestObjectId = Any.NonDefaultValueOf<Guid>(),
				Name = Any.NonNullValueOf<string>()
			}));
		}

		[Test]
		public void then_it_should_pass_when_used_with_matcher_that_matches_a_specific_check()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				TestObjectId = Some.ValueOf<Guid>(x => x != Guid.Empty)
			}));
		}

		[Test]
		public void then_it_should_fail_when_used_with_a_matcher_that_does_not_match_a_specific_check()
		{
		    var equalException = Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new TestObject
		    {
		        TestObjectId = Some.ValueOf<Guid>(x => x == Guid.Empty)
		    }));
		    equalException.Message.ShouldContain("Expected: Object matching (x == Guid.Empty)");
		}

		[Test]
		public void then_it_should_fail_when_used_with_a_not_default_matcher_that_does_not_match_a_specific_check()
		{
			Assert.Throws<EqualException>(() => new TestObject().ShouldLookLike(() => new TestObject
			{
				TestObjectId = Any.NonDefaultValueOf<Guid>()
			}))
			.Message.ShouldContain("Expected: Non-default value of System.Guid");
		}

		[Test]
		public void then_it_should_fail_when_used_with_a_not_null_matcher_that_does_not_match_a_specific_check()
		{
			Assert.Throws<EqualException>(() => new TestObject().ShouldLookLike(() => new TestObject
			{
				Name = Any.NonNullValueOf<string>()
			}))
			.Message.ShouldContain("Expected: Non-null value of System.String");
		}

		[Test]
		public void then_it_passes_when_a_value_for_a_nullable_type_matches_the_specified_check()
		{
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				OptionalDate = Some.ValueOf<DateTime>(d => d != DateTime.MinValue)
			}));
		}

        [Test]
		public void then_it_throws_when_a_null_value_for_a_nullable_type_is_not_allowed()
		{
			SUT.OptionalDate = null;
			Assert.Throws<EqualException>(() => SUT.ShouldLookLike(() => new TestObject
			{
				OptionalDate = Any.NonNullValueOf<DateTime?>()
			}));
		}

        [Test]
		public void then_it_passes_a_check_that_includes_more_complicated_logic()
		{
			SUT.OptionalDate = DateTime.Today.AddDays(-7);
			Assert.DoesNotThrow(() => SUT.ShouldLookLike(() => new TestObject
			{
				OptionalDate = DateTime.Today.AddDays(-7)
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
		public void then_it_fails_if_actual_nested_collection_is_empty()
		{
			SUT.NestedArray = new TestObject[0];
			Assert.Throws<EqualException>(() =>
			{
				SUT.ShouldLookLike(() => new TestObject
				{
					NestedArray = new[] {new TestObject {  Awesomeness = 10 } }
				});
			});
		}

		[Test]
		public void then_it_fails_if_actual_nested_collection_contains_more_than_expected_element()
		{
			SUT.NestedArray = new[]
			{
				new TestObject {Awesomeness = 10},
				new TestObject {Awesomeness = 11}
			};
			Assert.Throws<EqualException>(() =>
			{
				SUT.ShouldLookLike(() => new TestObject
				{
					NestedArray = new[]
					{
						new TestObject {Awesomeness = 10},
					}
				});
			});
		}

		[Test]
		public void then_it_fails_if_actual_array_is_empty()
		{
			var actual = new TestObject[0];
			Assert.Throws<EqualException>(() =>
			{
				actual.ShouldLookLike(() => new[] {new TestObject {Awesomeness = 10}});
			});
		}

		[Test]
		public void then_it_fails_if_actual_array_contains_more_than_expected_element()
		{
			var actual = new[]
				{
					new TestObject { Awesomeness = 10 },
					new TestObject { Awesomeness = 11 }
				};
			Assert.Throws<EqualException>(() =>
			{
				actual.ShouldLookLike(() => new[] {new TestObject {Awesomeness = 10}});
			});
		}

		[Test]
		public void then_it_passes_if_arrays_are_identical()
		{
			var actual = new[]
			{
				new TestObject { Awesomeness = 10 },
				new TestObject { Awesomeness = 11 }
			};
			Assert.DoesNotThrow(() =>
			{
				actual.ShouldLookLike(() => new[]
				{
					new TestObject { Awesomeness = 10 },
					new TestObject { Awesomeness = 11 }
				});
			});
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

		public class WidgetWithPrimitiveArray
		{
			public int[] Ints { get; set; }
		}

		[Test]
		public void then_it_passes_with_primitive_array_properties_that_are_the_same()
		{
			var actual = new WidgetWithPrimitiveArray {Ints = new[] {1, 2, 3}};
			actual.ShouldLookLike(() =>
				new WidgetWithPrimitiveArray
				{
					Ints = new[] {1, 2, 3}
				}
			);
		}
	}
}