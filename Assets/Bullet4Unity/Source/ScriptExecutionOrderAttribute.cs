using System;

[AttributeUsage(AttributeTargets.Class)]
public class ScriptExecutionOrderAttribute : Attribute {
    public int Order;
}
