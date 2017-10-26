namespace Microsoft.AAppCenterIngestion.Http
{
    public class TestServiceCall : ServiceCallDecorator
    {
        public TestServiceCall(IServiceCall decoratedApi) : base(decoratedApi)
        {
        }
    }
}
