namespace Conventions.Basics.Domain
{
    public class HelloConsumer
    {
        private readonly IHelloService _helloService;

        public HelloConsumer(IHelloService helloService)
        {
            _helloService = helloService;
        }

        public string GetHelloMessage()
        {
            return _helloService.SayHello();
        }
    }
}