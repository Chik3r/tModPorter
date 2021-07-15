using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using tModPorter.Rewriters;
using tModPorter.Rewriters.IdentifierRewriters;

namespace tModPorter.Tests.RewriterTests.SimpleIdentifierTests
{
    public sealed class ProjectileIdentifierRewriterTest : BaseIdentifierTest
    {
        protected override SimpleIdentifierRewriter CreateIdentifierRewriter(SemanticModel model,
            HashSet<(BaseRewriter rewriter, SyntaxNode originalNode)> nodeSet) => new ProjectileIdentifierRewriter(model, null, nodeSet);
    }
}