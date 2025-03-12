namespace PaymentGateway.Infra.External
{
    public class BaseApiConfiguration
    {
        public string BaseUrl { get; set; }
        public int TimeoutMs { get; set; }
        public int RetryCount { get; set; }
        public int RetryMs { get; set; }
    }
}