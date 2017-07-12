using System;
using UnityEditor;

[InitializeOnLoad]
public class ScriptOrderManager {

    static ScriptOrderManager() {
        foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
            if (monoScript.GetClass() != null) {
                foreach (var a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptExecutionOrderAttribute))) {
                    var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
                    var newOrder = ((ScriptExecutionOrderAttribute)a).Order;
                    if (currentOrder != newOrder)
                        MonoImporter.SetExecutionOrder(monoScript, newOrder);
                }
            }
        }
    }
}
