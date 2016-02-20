namespace Conventions.Basics.Domain
{
    public interface IHelloService
    {
        string SayHello();
    }

    public class HelloService : IHelloService
    {
        public string SayHello()
        {
            return "Hello from HelloService!";
        }
    }
}