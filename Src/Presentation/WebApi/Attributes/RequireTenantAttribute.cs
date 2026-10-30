namespace WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireTenantAttribute : Attribute;
}
