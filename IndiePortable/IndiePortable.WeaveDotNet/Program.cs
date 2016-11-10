
namespace IndiePortable.WeaveDotNet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis.CSharp;

    public class Program
    {
        public static void Main(string[] args)
        {
            string project = null;

            for (var n = 0; n < args.Length; n++)
            {
                if (args[n] == "/project" && args.Length > n + 1)
                {
                    project = args[n + 1];
                    break;
                }
            }

            Mono.Cecil

            if (project == null)
            {
                Console.WriteLine("Usage: IndiePortable.WeaveDotNet.exe /project <csproject>");
                return;
            }

            var xmldocument = XDocument.Load(project);
            var groups = from e in xmldocument.Root.Elements()
                         where e.Name == "ItemGroup"
                         select e;

            var compileItems = groups.Aggregate((IEnumerable<XElement>)new XElement[0], (acc, g) => acc.Concat(g.Elements().Where(e => e.Name == "Compile")));

            var projectDir = new DirectoryInfo(Path.GetDirectoryName(project));

            var syntaxTrees = new List<CSharpSyntaxTree>();
            foreach (var item in compileItems)
            {
                var tree = CSharpSyntaxTree.ParseText(
                    File.ReadAllText(
                        Path.Combine(
                            projectDir.FullName,
                            item.Attributes().First(a => a.Name.ToString() == "Include").Value))) as CSharpSyntaxTree;

                var cu = SyntaxFactory.CompilationUnit()
                                      .AddMembers(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("")));
                syntaxTrees.Add(tree);
            }


        }
    }
}
