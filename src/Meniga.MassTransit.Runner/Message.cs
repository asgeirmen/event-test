namespace Meniga.MassTransit.Runner
{
    public interface IEvent
    {
        string Text { get; set; }
    }
    public class EventTwo
    {
        public string Text { get; set; }
    }
}
