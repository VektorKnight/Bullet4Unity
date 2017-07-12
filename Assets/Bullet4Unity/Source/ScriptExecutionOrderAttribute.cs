using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ScriptExecutionOrderAttribute : Attribute {
    public int Order;

    public ScriptExecutionOrderAttribute() {

    }
}
