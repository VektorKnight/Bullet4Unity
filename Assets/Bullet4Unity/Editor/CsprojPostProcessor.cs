using SyntaxTree.VisualStudio.Unity.Bridge;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace VRResearchEditor {
    [InitializeOnLoad]
    public class CsprojPostProcessor : MonoBehaviour {
        static CsprojPostProcessor() {
            ProjectFilesGenerator.ProjectFileGeneration += ModifyProjectFile;
        }

        private static string ModifyProjectFile(string name, string content) {
            XDocument document = XDocument.Parse(content);

            var itemGroups = document.Root.Descendants()
                .Where(node => node.Name.LocalName == "ItemGroup");

            IEnumerable<XElement> shaderGroups = itemGroups.Select(group => {
                var child = group.Descendants()
                    .FirstOrDefault();

                if (child != null && child.Name.LocalName == "None")
                    return group;

                return null;
            });

            foreach (XElement group in shaderGroups) {
                group?.RemoveAll();
            }

            return document.ToString();
        }
    }
}