namespace Diploma.Controllers
{
	public interface IMessageProducer
	{
		void SendMessage<T>(T message);
	}
}
