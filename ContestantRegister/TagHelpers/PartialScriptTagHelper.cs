using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ContestantRegister.TagHelpers
{
    public class PartialScriptTagHelper : TagHelper
    {
        private IHttpContextAccessor ContextAccessor { get; }
        private HttpContext HttpContext => ContextAccessor.HttpContext;

        public PartialScriptTagHelper(IHttpContextAccessor contextAccessor)
        {
            ContextAccessor = contextAccessor;
        }

        public PartialScriptMode Mode { get; set; } = PartialScriptMode.Store;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            switch (Mode)
            {
                case PartialScriptMode.Store:
                    await StoreAsync(context, output);
                    break;
                case PartialScriptMode.Render:
                    Render(output);
                    break;
                default:
                    break;
            }
        }

        private async Task StoreAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.SuppressOutput();
            TagHelperContent child = await output.GetChildContentAsync();
            GetStorage().Add(child.GetContent());
        }

        private void Render(TagHelperOutput output)
        {
            output.SuppressOutput();
            foreach (string script in GetStorage())
            {
                output.Content.AppendHtml(script);
            }
        }

        private List<string> GetStorage()
        {
            List<string> storage = null;
            if (HttpContext.Items.ContainsKey(typeof(PartialScriptTagHelper)))
            {
                storage = HttpContext.Items[typeof(PartialScriptTagHelper)] as List<string>;
            }

            if (storage == null)
            {
                HttpContext.Items[typeof(PartialScriptTagHelper)] = storage = new List<string>();
            }

            return storage;
        }
    }

    public enum PartialScriptMode
    {
        Store,
        Render
    }
}
