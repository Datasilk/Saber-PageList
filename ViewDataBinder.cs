using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Saber.Core;

namespace Saber.Vendor.PageList
{
    /// <summary>
    /// Used to bind view html variables to platform data
    /// </summary>
    public class ViewDataBinder: IViewDataBinder
    {
        public List<ViewDataBinderModel> Bind()
        {
            //add custom html variable
            return new List<ViewDataBinderModel>{
                new ViewDataBinderModel() {
                    Key = "page-list",
                    Name = "Page List",
                    Description = "Display a list of webpages that belong to your website.",
                    Parameters = new Dictionary<string, ViewDataBinderParameter>()
                    {
                        {
                            "path",
                            new ViewDataBinderParameter()
                            {
                                Required = true,
                                DefaultValue = "",
                                Description = ""
                            }
                        },
                        {
                            "length",
                            new ViewDataBinderParameter()
                            {
                                DefaultValue = "4",
                                Description = "Total webpages to display within the list."
                            }
                        },
                        {
                            "recursive",
                            new ViewDataBinderParameter()
                            {
                                DefaultValue = "true",
                                Description = "If true, will include webpages from sub-folders in the list."
                            }
                        },
                        {
                            "container-file",
                            new ViewDataBinderParameter()
                            {
                                DefaultValue = "/Vendors/PageList/container.html",
                                Description = "Allows the user to define a custom html template for the page-list container, which includes paging buttons."
                            }
                        },
                        {
                            "item-file",
                            new ViewDataBinderParameter()
                            {
                                DefaultValue = "/Vendors/PageList/page.html",
                                Description = "Allows the user to define a custom html template for an item within the page-list, which includes the title & description of a webpage, the author & creation date, along with a thumbnail image."
                            }
                        },
                    },
                    Callback = new Func<View, IRequest, Dictionary<string, string>, string, string, List<KeyValuePair<string, string>>>((view, request, data, prefix, key) =>
                    {
                        var results = new List<KeyValuePair<string, string>>();
                        /* data /////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                        if (data.ContainsKey("container-file"))
                        {
                            //get custom container template
                            containerTemplate = data["container-file"];
                        }
                        if (data.ContainsKey("item-file"))
                        {
                            //get custom item template
                            itemTemplate = data["item-file"];
                        }
                        else
                        {
                            //include css file for page lists
                            request.AddCSS("/css/vendors/pagelist/pagelist.css", "pagelist");
                        }

                        if (data.ContainsKey("recursive"))
                        {
                            try
                            {
                                recursive = bool.Parse(data["recursive"]);
                            }
                            catch (Exception) { }
                        }
                        var container = new View(containerTemplate);
                        var item = new View(itemTemplate);
                        var files = new string[] { };
                        var path = "";
                        var length = 4;
                        if (data.ContainsKey("path"))
                        {
                            path = data["path"];
                        }
                        if (data.ContainsKey("length"))
                        {
                            int.TryParse(data["length"], out length);
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
