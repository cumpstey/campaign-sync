# Campaign Sync

## Background

### The problem with Campaign

Having worked on our client's web project for a while, we took over their CRM project at a time when they were upgrading from Neolane v5 to Adobe Campaign v6.1.

I'm a web developer, used to decent development tools, and was somewhat depressed at the state of the development tools available in Campaign - no automated backups; no intellisense; even the syntax highlighting doesn't work properly. It was all made worse by the fact that the only way to access it was on a remote desktop only available through the client's vpn, with Notepad as the only text editor, and the only way to transfer files being to email them. The prospect was not good.

In addition, it proved to be impossible to transfer images from one instance of Campaign to another - `xtk:fileRes` records yes, in a package, but absolutely not the associated files. This led us to just accepting for a long time that we had no images in the Dev environment, and so couldn't properly test anything. 

A major new feature, which was in large part our client's reason for the upgrade, was the use of the content management functionality to allow the users to build deliveries populated with their own content. We now have a solution with a single JavaScript template used for every delivery sent out - for easy maintenance - sitting on top of a lot of nicely-structured back-end JavaScript, implementing IoC, and with a view model factory to translate the XML data to pure JS objects used in the views, and with a complete set of unit tests. This clearly needs decent development tools!

### The solution...?

Having discovered that virtually all functionality in Campaign was available through SOAP endpoints, I realised that it would be possible to build something akin to the working environment I was used to as a web developer - using the decent code editors in my local dev environment, storing files in source control, and having the kind of release process for new features which automated as much as possible, and which enabled simple rollback.

This whole project is built entirely on my own ideas as to how Campaign development should be done. It is _not_ built on any best-practice recommendations or similar ideas from Adobe themselves, or anyone else - basically because I've failed to find any. Compared to the kind of documentation and community you get when working on, say, Umbraco or EPiServer, the support available on Campaign is minimal - I've been on one fairly trivial training session, looked at a bit of unsearchable, and very incomplete, documentation on the web, and used the JS API documentation to find method signatures, extrapolating any further information about what the method does and what kind of parameters it expects, as in most cases this information is completely missing. Adobe Support have been helpful - ranging from providing real solutions, to suggesting inspired workarounds to what are clearly bugs.

## Entity support

The app allows the download and upload of the following entities:

- `nms:includeView` - Personalisation block (text part only).
- `nms:publishing` - Publishing model.
- `xtk:form` - Input form.
- `xtk:javascript` - JavaScript code.
- `xtk:jst` - JavaScript template.
- `xtk:srcSchema` - Data schema. The `xtk:schema` is rebuilt for each `xtk:srcSchema` uploaded.
- Images, as `xtk:fileRes` with associated image file (upload only).

More will be added as and when we need them.

## Usage

Example commands for the various usages are below. Descriptions of the available parameters can be found by running:

```dos
> CampaignSync.exe --help
```

### Download.

To download all JavaScript templates with namespace 'cus' and name starting with 'myTemplate' into the folder 'C:\Campaign files\xtk_jst\cus':

```dos
> CampaignSync.exe -m Download -s http://neolane.net -u myuser -p mypassword --dir "C:\Campaign files" --schema xtk:jst --conditions "@namespace='cus'" "@name like 'myTemplate%'"
```

### Upload

To upload all files which have metadata specified in the appropriate syntax from: `C:\Campaign files\xtk_javascript\cus\myscript.js`; any file with the .jssp extension within `C:\Campaign files\xtk_jst` and chid folders; and any file within `C:\Campaign files\xtk_form` or child folders:

```dos
> CampaignSync.exe -m Upload -s http://neolane.net -u myuser -p mypassword --files "C:\Campaign files\xtk_javascript\cus\myscript.js" "C:\Campaign files\xtk_jst\*.jssp" "C:\Campaign files\xtk_form"
```

Text in the files can be replaced with alternative text on upload. We use this as we have a secondary database which Campaign connects to, which has a different name on each environment.

```dos
> CampaignSync.exe -m Dpload -s http://neolane.net -u myuser -p mypassword --files "C:\Campaign files\xtk_srcSchema\cus\*.xml"  --replacements "DevDB=>ProdDB" "DevSetting=>LivSetting"
```

### Image upload

Images must have an associated `imageData.csv` metadata file, which covers all images in a folder, containing: name of the folder to which the image should be uploaded; internal name; label; alt. These files can be generated, listing all images in each directory, for a directory tree by the following command; you then need to fill in the metadata before uploading.

```dos
> CampaignSync.exe -m GenerateImageData --dirs "C:\Campaign files\images" --recursive
```

The images can then be uploaded by:

```dos
> CampaignSync.exe -m ImageUpload -s http://neolane.net -u myuser -p mypassword --files "C:\Campaign files\images"
```

Images are created, as far as I can tell, in the same way as they would be if manually uploaded in Campaign: same location on disk, and same properties in the `xtk:fileRes` record.

In order to use the image upload functionality, the `zon:persist` JS and schema and `zon:common` JS files will need to be uploaded to Campaign. These provide a `zon:persist#WriteImage` SOAP endpoint.

## SOAP Services

Wrappers are provided for the following built-in endpoints:

- `xtk:builder#BuildSchemaFromId`
- `xtk:session#Logon`
- `xtk:persist#Write`
- `xtk:persist#WriteCollection`
- `xtk:queryDef#ExecuteQuery`

It's easy enough to add a wrapper for another, or a custom, endpoint.

Also included are:

- Wrapper for the custom `zon:persist#WriteImage` endpoint, used for uploading images.
- Alternative wrappers for `xtk:persist#Write` and `xtk:queryDef#ExecuteQuery` requests. These make request to custom endpoints (`zon:persist#WriteZip` and `zon:queryDef#ExecuteQueryZip` - schema and JS provided) with the content of the query zipped and base64 encoded. This may help to get round length and keyword filters in a firewall.

## Upcoming features

Features which we aim to include soon:

- Upload support of `nms:includeView` and `nms:publishing`.
- Moving the hardcoded filepaths in a couple of the JS functions into `xtk:option`.
- Generic workflow trigger.
- Workflow which regenerates deliveries which use a JS template, which can be triggered when the template is updated so the deliveries can use the latest version without manual regeneration of each in the ui.
- Removing irrelevant elements and attributes from the `xtk:form`, `xtk:srcSchema` and other types deriving from `xtk:entity` on download.

## Further notes

The `xtk:session#Logon` method requires credentials to be sent in plain text. Passing plain text credentials over http is clearly a security issue. I recommend you use https, or some other mechanism, to avoid this.

The Campaign metadata which identifies which entity in Campaign a file corresponds to - schema, namespace, name, label - is stored in a comment in the file. This is written on download, and read and removed on upload - it doesn't appear in the file in Campaign itself.

We use the `.jssp` (ie. JavaScript Server Page) file extension for JavaScript templates. No code editor I've found has properly decent support for this. Notepad++ HTML syntax highlighting works fairly well, but misses things; and JSP isn't satisfactory for those cases where JS syntax differs from Java syntax. Atom does well with its JavaServer Pages grammar. Sublime does slightly less well with its Java Server Page (JSP) syntax, and although I've not done it it looks like you can probably set a default for a file extension. There's no built-in language in VS Code which does a good job. To set JavaServer Pages as the default grammer for `.jssp` in Atom, add this to the config:

```coffeescript
core:
  customFileTypes: {
    "text.html.jsp": ["jssp"]
  }
```

Actually in our particular case we use `.jssp` for the JS template for the text version of the email only; and `.html` for the html version. The app does an additional transform step on files with the `.html` extension. I developed a syntax which allows us to hold both static example content, so the front end developers can build, demonstrate and test their markup, and the code which adds in the dynamic content in a single file. I'll document that syntax here in the future.
