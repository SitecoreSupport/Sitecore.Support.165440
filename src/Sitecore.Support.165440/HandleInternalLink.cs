namespace Sitecore.Support.Modules.EmailCampaign.Core.Pipelines.GenerateLink.Hyperlink
{
    using Sitecore.Diagnostics;
    using Sitecore.Links;
    using Sitecore.Modules.EmailCampaign.Core;
    using Sitecore.Modules.EmailCampaign.Core.Links;
    using Sitecore.Modules.EmailCampaign.Core.Pipelines.GenerateLink;
    using Sitecore.Sites;
    using System;
    using System.Text.RegularExpressions;

    public class HandleInternalLink : GenerateLinkProcessor
    {
        public override void Process(GenerateLinkPipelineArgs args)
        {
            Assert.IsNotNull(args, "Arguments can't be null");
            Assert.IsNotNull(args.Url, "Url link can't be null");
            if (LinksManager.IsAbsoluteLink(args.Url))
            {
                if (args.Url == "link:")
                {
                    args.AbortPipeline();
                }
            }
            else if (Regex.IsMatch(args.Url, "^([a-zA-Z0-9+.-]+:)"))
            {
                args.AbortPipeline();
            }
            else
            {
                DynamicLink link;
                if ((args.Url.IndexOf("~/link.aspx?", StringComparison.InvariantCulture) >= 0) && DynamicLink.TryParse(args.Url, out link))
                {
                    UrlOptions defaultUrlOptions = LinkManager.GetDefaultUrlOptions();
                    defaultUrlOptions.SiteResolving = true;
                    defaultUrlOptions.Site = SiteContext.GetSite(args.WebsiteConfigurationName);
                    
                    //Patch 165440
                    defaultUrlOptions.Language = args.MailMessage.TargetLanguage;
                    //Patch 165440 end

                    args.Url = LinkManager.GetItemUrl(new ItemUtilExt().GetItem(link.ItemId), defaultUrlOptions);
                }
                if (args.Url.IndexOf('/') != 0)
                {
                    args.Url = args.Url.Insert(0, "/");
                }
            }
        }
    }
}
