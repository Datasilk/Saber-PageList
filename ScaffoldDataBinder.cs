using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Saber.Common.Platform
{
    /// <summary>
    /// Used to bind scaffold html variables to platform data
    /// </summary>
    public partial class ScaffoldDataBinder
    {
        partial void Bind()
        {
            HtmlVars.Add("page-list", new Func<Datasilk.Web.Request, string, string>((request, data) =>
            {
                var args = GetMethodArgs(data);
                /* args /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                 * 
                 * path: (required) relative path from "/Content/pages/" used to list pages from
                 * length: (optional) the amount of pages to display before showing paging buttons at the bottom of the list
                 * recursive: (optional) recurse through sub-folders to find more pages
                 * container-file: (optional) change the default scaffold template used to render the list container & paging buttons
                 * item-file: (optional) change the default scaffold template used to render each page list item
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
                var container = new Scaffold(containerTemplate);
                var item = new Scaffold(itemTemplate);
                var files = new string[] { };
                if (args.ContainsKey("path"))
                {
                    files = Directory.GetFiles(Server.MapPath("/Content/pages/" + args["path"]), "*.html", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                }

                if(files.Length > 0)
                {
                    files = files.Select(f =>
                    {
                        //clean up file paths
                        var paths = f.Split('\\').ToList();
                        var index = paths.FindIndex(i => i == "Content");
                        if(index > 0)
                        {
                            paths = paths.Skip(index).ToList();
                        }
                        return "/" + string.Join('/', paths);
                    }).ToArray();
                }

                //get config files for each page
                var html = new StringBuilder();
                foreach(var file in files)
                {
                    if (file.IndexOf("template.html") > 0) { continue; }
                    var config = PageInfo.GetPageConfig(file);
                    item.Data["title"] = config.title.body;
                    item.Data["date-created"] = config.datecreated.ToShortDateString();
                    item.Data["thumb"] = config.thumbnail;
                    item.Data["description"] = config.description;
                    html.Append(item.Render() + "\n");
                }

                container.Data["list"] = html.ToString();

                //TODO: paging buttons

                return container.Render();
            }));
        }
    }
}
