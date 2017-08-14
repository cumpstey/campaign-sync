/*!
Adobe Campaign metadata:
Schema: xtk:javascript
Name: zon:persist.js
Label: Persist SOAP endpoints
!*/
loadLibrary("/nl/core/shared/nl.js");

NL.require("zon:common.js");

/**
 * Wrapper for the xtk:persist#Write endpoint, which unencodes submitted data
 */
function zon_persist_WriteZip(input) {
  var queryXml = NL.ZON.unzipAsXml(input, "WriteZip");
  xtk.session.Write(queryXml);
}

/**
 * Wrapper for the xtk:persist#WriteCollection endpoint, which unencodes submitted data
 */
function zon_persist_WriteCollectionZip(input) {
  var queryXml = NL.ZON.unzipAsXml(input, "WriteCollectionZip");
  xtk.session.WriteCollection(queryXml);
}

/**
 * Saves an image received base64 encoded as an xtk:fileRes
 */
function zon_persist_WriteImage(input) {
  // Global parameters
  var uploadDirPath = getOption("zon_UploadDirectory");
  var publishDirPath = getOption("zon_FileResPublishDirectory");

  // Check folder exists
  var folderId = NL.ZON.getFolderIdByName(input["@folderName"]);
  if (!folderId.toString()) {
    logError("Folder '" + input["@folderName"] + "' doesn't exist");
    return;
  }

  // TODO even if this mostly works, it's not a good way to get the file extension.
  // It won't work, or at leats won't pick out the correct extension, if there's a . in the filename itself.
  var extension = input.fileContent["@fileName"].toString().split('.')[1];
  var md5 = input.fileContent["@md5"];
  var filePath = uploadDirPath + md5 + '.' + extension;
  var publishFilePath = publishDirPath + md5 + '.' + extension;

  //Delete file if it exists
  var existingFile = new File(filePath);
  if (existingFile.exists) {
    existingFile.remove();
  }

  existingFile.dispose();

  // Create file
  var buffer = new MemoryBuffer();
  buffer.appendBase64(input["fileContent"]);
  buffer.save(filePath);
  buffer.dispose();

  // Check whether fileRes already exists
  var existingFileResId = NL.ZON.getFileResIdByName(input.fileRes["@internalName"]);
  if (existingFileResId > 0)
  {
    // Update an existing record
    var fileRes = xtk.fileRes.load(existingFileResId);
    fileRes.label = input.fileRes["@label"];
    fileRes.originalName = uploadDirPath + input.fileContent["@fileName"];
    fileRes.fileName = publishDirPath + input.fileContent["@fileName"];
    fileRes.md5 = md5;
    fileRes.useMd5AsFilename = 1;
    fileRes.publish = 0;
    fileRes.contentType = input.fileContent["@mimeType"];
    fileRes.alt = input.fileRes["@alt"];
    fileRes.width = input.fileRes["@width"];
    fileRes.height = input.fileRes["@height"];
    fileRes.save();
    fileRes.PublishIfNeeded();

    // Set the folder.
    // TODO: probably should rewrite the update and create fucntions in this style...
    xtk.session.Write({
      fileRes: {
        xtkschema: "xtk:fileRes",
        internalName: input.fileRes["@internalName"].toString(),
        folder: {
          id: folderId.toString()
        },
      }
    });
  }
  else
  {
    // Create the file resource
    var fileResXml = <fileRes
                codepage='0'
                internalName={input.fileRes["@internalName"]}
                label={input.fileRes["@label"]}
                originalName={uploadDirPath + input.fileContent["@fileName"]}
                fileName={publishDirPath + input.fileContent["@fileName"]}
                md5={md5}
                storageType='5'
                useMd5AsFilename='1'
                publish='0'
                userContentType='0'
                contentType={input.fileContent["@mimeType"]}
                alt={input.fileRes["@alt"]}
                width={input.fileRes["@width"]}
                height={input.fileRes["@height"]}>
                <folder id={folderId} />
              </fileRes>;
    var fileRes = xtk.fileRes.create(fileResXml);
    fileRes.save();
    fileRes.PublishIfNeeded();
  }
}
