using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using tModPorter.Rewriters;
using tModPorter.Rewriters.AssignmentRewriters;
using Xunit;

namespace tModPorter.Tests
{
    public class DamageClassTest
    {
        private const string VisitNodeAddNodes_Data01 = @"
Item item = new Item();
item.magic = true;
";
        private const string VisitNodeAddNodes_Data02 = @"
Item item = new Item();
item.melee = true;
item.summon = false;
";

        private const string VisitNodeAddNodes_Data03 = @"
Item item = new Item();
item.width = 40;
";

        [Theory]
        [InlineData(VisitNodeAddNodes_Data01, 1)]
        [InlineData(VisitNodeAddNodes_Data02, 2)]
        [InlineData(VisitNodeAddNodes_Data03, 0)]
        public void VisitNodeAddNodes(string input, int numNodesToFind)
        {
            Utils.CreateCSharpCompilation(input, "DamageClassTest", out _,
                out CompilationUnitSyntax root, out SemanticModel model);

            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet = new();
            DamageClassRewriter damageClassRewriter = new(model, new List<string>(), nodeSet);

            IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes().Where(x => x is AssignmentExpressionSyntax);

            foreach (SyntaxNode assigmentNode in assigmentNodes)
                damageClassRewriter.VisitNode(assigmentNode);
            
            Assert.Equal(nodeSet.Count, numNodesToFind);
        }
        
        private const string VisitNodeAddNodes_InvalidType_Data01 = @"
Item item = new Item();
item.melee;
";
        private const string VisitNodeAddNodes_InvalidType_Data02 = @"
Item item = new Item();
melee = 2;
";

        [Theory]
        [InlineData(VisitNodeAddNodes_InvalidType_Data01)]
        [InlineData(VisitNodeAddNodes_InvalidType_Data02)]
        public void VisitNodeAddNodes_WrongStatementType(string input)
        {
            Utils.CreateCSharpCompilation(input, "DamageClassTest", out _,
                out CompilationUnitSyntax root, out SemanticModel model);

            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet = new();
            DamageClassRewriter damageClassRewriter = new(model, new List<string>(), nodeSet);

            IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes();

            foreach (SyntaxNode assigmentNode in assigmentNodes)
                damageClassRewriter.VisitNode(assigmentNode);
            
            Assert.Empty(nodeSet);
        }

        [Fact]
        public void RewriterTypeGet()
        {
            DamageClassRewriter rewriter = new(null, null, null);
            Assert.Equal(RewriterType.Assignment, rewriter.RewriterType);
        } 
        
        private const string RewriteNode_Source01 = @"
Item item = new Item();
item.melee = true;
";
        private const string RewriteNode_Target01 = @"
Item item = new Item();
item.DamageType = DamageClass.Melee;
";

        [Theory]
        [InlineData(RewriteNode_Source01, RewriteNode_Target01)]
        public void RewriteNodeTest(string source, string target)
        {
            Utils.CreateCSharpCompilation(source, "DamageClassTest", out _,
                out CompilationUnitSyntax root, out SemanticModel model);

            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet = new();
            DamageClassRewriter damageClassRewriter = new(model, new List<string>(), nodeSet);

            IEnumerable<SyntaxNode> assigmentNodes = root.DescendantNodes();

            foreach (SyntaxNode assigmentNode in assigmentNodes)
                damageClassRewriter.VisitNode(assigmentNode);

            foreach ((_, SyntaxNode originalNode) in nodeSet)
                root = root.ReplaceNode(originalNode, damageClassRewriter.RewriteNode(originalNode));

            Assert.Equal(target, root.ToFullString());
        }
    }
}