﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Saber.Common.Platform;

namespace Saber.Vendor.PageList
{
    /// <summary>
    /// Used to bind view html variables to platform data
    /// </summary>
    public partial class ViewDataBinder: Common.Platform.ViewDataBinder
    {
        public override void Bind()
        {
            //add custom html variable
            HtmlVars.Add("page-list", new ViewDataBinderModel() {
                Name = "Page List",
                Description = "Display a list of webpages that belong to your website.",
                Parameters = new Dictionary<string, ScaffoldDataBinderParameter>()
                {
                    {
                        "path",
                        new ScaffoldDataBinderParameter()
                        {
                            Required = true,
                            DefaultValue = "",
                            Description = ""
                        }
                    },
                    {
                        "length",
                        new ScaffoldDataBinderParameter()
                        {
                            DefaultValue = "10",
                            Description = "Total webpages to display within the list."
                        }
                    },
                    {
                        "recursive",
                        new ScaffoldDataBinderParameter()
                        {
                            DefaultValue = "true",
                            Description = "If true, will include webpages from sub-folders in the list."
                        }
                    },
                    {
                        "container-file",
                        new ScaffoldDataBinderParameter()
                        {
                            DefaultValue = "/Vendor/PageList/container.html",
                            Description = "Allows the user to define a custom html template for the page-list container, which includes paging buttons."
                        }
                    },
                    {
                        "item-file",
                        new ScaffoldDataBinderParameter()
                        {
                            DefaultValue = "/Vendor/PageList/page.html",
                            Description = "Allows the user to define a custom html template for an item within the page-list, which includes the title & description of a webpage, the author & creation date, along with a thumbnail image."
                        }
                    },
                },
                Callback = new Func<Datasilk.Core.Web.Request, string, string, List<KeyValuePair<string, string>>>((request, data, prefix) =>
                {
                    var results = new List<KeyValuePair<string, string>>();
                    var args = GetMethodArgs(data);
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
                    var containerTemplate = "/Vendor/PageList/container.html";
                    var itemTemplate = "/Vendor/PageList/page.html";
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
                        request.AddCSS("/css/vendor/pagelist/pagelist.css", "pagelist");
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
                    if (args.ContainsKey("path"))
                    {
                        files = Directory.GetFiles(Server.MapPath("/Content/pages/" + args["path"]), "*.html", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    }

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
                    foreach (var file in files)
                    {
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
                    results.Add(new KeyValuePair<string, string>("page-list", container.Render()));
                    return results;
                })
            });
        }
    }
}