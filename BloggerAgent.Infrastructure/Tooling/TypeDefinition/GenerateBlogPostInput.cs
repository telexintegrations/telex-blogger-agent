using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Infrastructure.Tooling.Types
{
    public class GenerateBlogPostInput
    {
        [Description("The title or main topic of the blog post.")]
        public string Title { get; set; }

        [Description("Industry context to shape the tone or examples.")]
        public string Industry { get; set; }

        [Description("Preferred tone, e.g., professional, casual, persuasive.")]
        public string Tone { get; set; }
    }
   

}
