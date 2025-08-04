namespace Pi.GlobalEquities.Services;

public interface IOrderReferenceValidator
{
    OrderTagInfo Extract(string clientTag, string providerAccountId);
}
