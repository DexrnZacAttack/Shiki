namespace Shiki.Common.Factory;

/// <summary>
/// Automatically creates a static CreateInstance method which redirects to the applied constructor 
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public class FactoryConstructableAttribute : Attribute;