# PageList
A vendor plugin for the [Saber](https://github.com/Datasilk/Saber) CMS platform that can display a list of web pages that belong to a Saber website.

### Prerequisites
* Visual Studio 2017
* Clone [Saber](https://github.com/Datasilk/Saber) repository

### Installation
* Clone this repository inside your Saber project within `/App/Vendor/` and name the folder **PageList**
	> NOTE: use `git clone` instead of `git submodule add` since the contents of the Vendor folder is ignored by git
* Run `gulp default`

### Usage
To use a page list within your Saber website, log into your website and access Saber's **Editor UI** by pressing the *Esc* key. Then, within your webpage *html* file, add the following example:

```
{{page-list path:"", length:"4"}}
```
This will display a vertical list of webpages that your users can navigate to.

### Properties

|property|required|default|description|
|---|---|---|---|
|path|true|""|The relative path after `/App/Content/pages/` to load webpages from|
|length|false|"10"|The amount of webpages to display in the list before showing paging buttons at the bottom of the list|
|recursive|false|"true"|Recurse through subfolders to find even more webpages to load into the list|
|container-file|false|"/Vendor/PageList/container.html"|Relative path to the **View** used to load the container of the entire list, which includes optional paging buttons|
|item-file|false|"/Vendor/PageList/page.html"|Relative path to the **View** used to load each item in the list of webpages.

### Custom Layout
Use the **container-file** and **item-file** properties to specify your own html template used to render the list of webpages. 

----

Below is a list of variables you can add to your html template to display information about the each webpage within the **item-file**.

|variable|description|
|---|---|
|{{url}}|The relative path to the webpage, which can be used within an anchor link, e.g. `<a href="{{url}}"></a>`|
|{{title}}|Title of the webpage (without a prefix or suffix)|
|{{description}}|A short summary of the webpage|
|{{thumb}}|Thumbnail image associated with the webpage|
|{{author}}|The full name of the user who created the webpage|
|{{date-created}}|The `MM/d/yyyy` that the page was created|

----

Below is a list of html variables used for loading the page list & paging buttons within the **container-file**.

|variable|description|
|---|---|
|{{id}}|A unique ID used to identify the list when calling paging functions|
|{{list}}|Replaced with the rendered list of web pages|
|{{has-prev}} {{/has-prev}}|Used to show/hide the *previous* paging button|
|{{has-next}} {{/has-next}}|Used to show/hide the *next* paging button|
|{{paging-prev}}|a JavaScript function used to load the previous list of pages|
|{{paging-next}}|a JavaScript function used to load the next list of pages|

----

Developed by [Mark Entingh](https://www.markentingh.com), who is the creator of Saber and [Datasilk](https://www.datasilk.io).