using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Saber.Core;
using Saber.Vendor;

namespace Saber.Vendors.PageList
{
    /// <summary>
    /// Used to bind view html variables to platform args
    /// </summary>
    public class HtmlComponents: IVendorHtmlComponents
    {
        public List<HtmlComponentModel> Bind()
        {
            //add custom html variable
            return new List<HtmlComponentModel>{
                new HtmlComponentModel() {
                    Key = "page-list",
                    Name = "Page List",
                    Icon = "Vendors/PageList/icon.svg",
                    Description = "Display a list of webpages that belong to your website.",
                    Parameters = new Dictionary<string, HtmlComponentParameter>()
                    {
                        {
                            "path",
                            new HtmlComponentParameter()
                            {
                                Name = "Path",
                                Required = true,
                                DefaultValue = "",
                                Description = "The relative path to search for web pages within your website."
                            }
                        },
                        {
                            "length",
                            new HtmlComponentParameter()
                            {
                                Name = "Length",
                                DataType = HtmlComponentParameterDataType.Number,
                                DefaultValue = "",
                                Description = "Total webpages to display within the list."
                            }
                        },
                        {
                            "recursive",
                            new HtmlComponentParameter()
                            {
                                Name = "Recursive",
                                DataType = HtmlComponentParameterDataType.Boolean,
                                DefaultValue = "true",
                                Description = "If true, will include webpages from sub-folders in the list."
                            }
                        },
                        {
                            "container-file",
                            new HtmlComponentParameter()
                            {
                                Name = "Container File",
                                DefaultValue = "",
                                Description = "Relative path to your custom html template used as the container for list items and can include paging buttons."
                            }
                        },
                        {
                            "item-file",
                            new HtmlComponentParameter()
                            {
                                Name = "Item File",
                                DefaultValue = "",
                                Description = "Relative path to your custom html template for a list item, which includes the title & description of a webpage, the author & creation date, along with a thumbnail image."
                            }
                        },
                    },
                    Render = new Func<View, IRequest, Dictionary<string, string>, string, string, string, List<KeyValuePair<string, string>>>((view, request, args, data, prefix, key) =>
                    {
                        var results = new List<KeyValuePair<string, string>>();
                        /* args /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                         * 
                         * path: (required) relative path from "/Content/pages/" used to list pages from
                         * length: (optional) the amount of pages to display before showing paging buttons at the bottom of the list
                         * recursive: (optional) recurse through sub-folders to find more pages
                         * container-file: (optional) change the default view template used to render the list container & paging buttons
                         * item-file: (optional) change the default view template used to render each page list item
                         * 
                         *///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //get list of pages to load
                        var containerTemplate = "/Vendors/PageList/container.html";
                        var itemTemplate = "/Vendors/PageList/page.html";
                        var recursive = true;
                        if (args.ContainsKey("container-file"))
                        {
                            //get custom container template
                            containerTemplate = args["container-file"];
                        }
                        if (args.ContainsKey("item-file"))
                        {
                            //get custom item template
                            itemTemplate = args["item-file"];
                        }
                        else
                        {
                            //include css file for page lists
                            request.AddCSS("/css/vendors/pagelist/pagelist.css", "pagelist");
                        }

                        if (args.ContainsKey("recursive"))
                        {
                            try
                            {
                                recursive = bool.Parse(args["recursive"]);
                            }
                            catch (Exception) { }
                        }
                        var container = new View(containerTemplate);
                        var item = new View(itemTemplate);
                        var files = new string[] { };
                        var path = "";
                        var length = 4;
                        if (args.ContainsKey("path"))
                        {
                            path = args["path"];
                        }
                        if (args.ContainsKey("length"))
                        {
                            int.TryParse(args["length"], out length);
                        }
                        files = Directory.GetFiles(App.MapPath("/Content/pages/" + path), "*.html", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                        if (files.Length > 0)
                        {
                            files = files.Select(f =>
                            {
                                //clean up file paths
                                var paths = f.Split('\\').ToList();
                                var index = paths.FindIndex(i => i == "Content");
                                if (index > 0)
                                {
                                    paths = paths.Skip(index).ToList();
                                }
                                return "/" + string.Join('/', paths);
                            }).ToArray();
                        }

                        //get config files for each page
                        var html = new StringBuilder();
                        var i = 0;
                        foreach (var file in files)
                        {
                            i++;
                            if(i > length){break; }
                            if (file.IndexOf("template.html") > 0) { continue; }
                            var config = PageInfo.GetPageConfig(file);
                            item["title"] = config.title.body;
                            item["date-created"] = config.datecreated.ToShortDateString();
                            item["thumb"] = config.thumbnail;
                            item["description"] = config.description;
                            html.Append(item.Render() + "\n");
                        }

                        container["list"] = html.ToString();

                        //TODO: paging buttons
                        results.Add(new KeyValuePair<string, string>(prefix + key, container.Render()));
                        return results;
                    })
                }
            };
        }
    }
}
