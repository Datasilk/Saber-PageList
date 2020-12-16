using Saber.Vendor;

namespace Saber.Vendors.PageList
{
    public class Info : IVendorInfo
    {
        public string Key { get; set; } = "PageList";
        public string Name { get; set; } = "Page List";
        public string Description { get; set; } = "Allows users to add a list of URL links to their web page that link to other pages within their website.";
        public Version Version { get; set; } = "1.0.0.0";
    }
}
