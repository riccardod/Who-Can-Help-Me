namespace WhoCanHelpMe.Web.Cms.Pages
{
    using N2;
    using N2.Details;
    using N2.Installation;
    using N2.Integrity;
    using Web.Cms;

    /// <summary>
    /// The site root definition
    /// </summary>
    [PageDefinition("Site Root",
        Description = "A site root used to organize home pages.",
        SortOrder = 0,
        InstallerVisibility = InstallerHint.PreferredRootPage,
        IconUrl = "~/n2/resources/icons/page_gear.png")]
    [WithEditableTitle("Title", 5, Focus = true, ContainerName = Tabs.Content)]
    [RestrictParents(AllowedTypes.None)]
    public class SiteRoot : AbstractPage
    {
        /// <summary>
        /// Gets site root Url.
        /// </summary>
        /// <value>
        /// The site root url.
        /// </value>
        public override string Url
        {
            get { return "~/"; }
        }
    }
}