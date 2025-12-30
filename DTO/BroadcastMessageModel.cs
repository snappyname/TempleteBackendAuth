namespace DTO
{
    public class BroadcastMessageModel<T>
    {
        public string Type { get; set; }
        public T Payload { get; set; }
    }
}
