using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor.Helpers.Web.Mvc;
using SpecsFor.Mvc.Demo.Controllers;

namespace SpecsFor.Mvc.Specs.Controllers
{
	public class IsAjaxRequestControllerSpecs : SpecsFor<IsAjaxRequestController>
	{
		protected override void When()
		{	
			base.When();
			this.UseFakeContextForController();
		}
		
		[Test]
		public void when_posting_to_index_normally_result_is_viewresult()
		{
			//Act
			var result = SUT.Index(It.IsAny<string>());

			//Assert
			result.ShouldBeType(typeof (ViewResult));
		}

		[Test]
		public void when_posting_to_index_through_ajax_result_is_jsonresult()
		{
			//Arrange
			this.FakeAjaxRequest();

			//Act
			var result = SUT.Index(It.IsAny<string>());

			//Assert
			result.ShouldBeType(typeof(JsonResult));
		}
	}
}
