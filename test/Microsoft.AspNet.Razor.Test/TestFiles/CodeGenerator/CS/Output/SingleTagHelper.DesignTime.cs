namespace TestOutput
{
    using Microsoft.AspNet.Razor.Runtime.TagHelpers;
    using System;
    using System.Threading.Tasks;

    public class SingleTagHelper
    {
        private static object @__o;
        private void @__RazorDesignTimeHelpers__()
        {
            #pragma warning disable 219
            string __tagHelperDirectiveSyntaxHelper = null;
            __tagHelperDirectiveSyntaxHelper = 
#line 1 "SingleTagHelper.cshtml"
              "something"

#line default
#line hidden
            ;
            #pragma warning restore 219
        }
        #line hidden
        private PTagHelper __PTagHelper = null;
        #line hidden
        public SingleTagHelper()
        {
        }

        #pragma warning disable 1998
        public override async Task ExecuteAsync()
        {
            __PTagHelper = CreateTagHelper<PTagHelper>();
            __PTagHelper.Age = 1337;
        }
        #pragma warning restore 1998
    }
}
