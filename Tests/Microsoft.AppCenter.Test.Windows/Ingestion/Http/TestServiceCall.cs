namespace Microsoft.AppCenter.Ingestion.Http
{
    public class TestServiceCall : ServiceCallDecorator
    {
        public TestServiceCall(IServiceCall decoratedApi) : base(decoratedApi)
        {
        }
    }
}
