namespace Pi.GlobalEquities.Services;

public interface IOrderReferenceIssuer
{
    string CreateClientTag(OrderTagInfo orderTagInfo);
}
