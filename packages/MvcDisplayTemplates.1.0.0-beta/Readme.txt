This package has installed Razor versions of the default ASP.NET MVC DisplayTemplates with one minor change:
the templates now have a layout file that wraps the display in a span that includes the ID of the property
being rendered.  This makes it possible for testing frameworks, such as SpecsFor.Mvc, to identify content
on the page.  The templates also give you a great starting point for establishing your own rendering 
conventions.  

For more information, check out my blog at http://trycatchfail.com!

---Matt Honeycutt