using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ScriptExecutionOrderAttribute : Attribute {
    public int Order;

    public ScriptExecutionOrderAttribute() {

    }
}
