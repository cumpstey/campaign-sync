/*!
Adobe Campaign metadata:
Schema: xtk:javascript
Name: zon:persist.js
Label: Persist SOAP endpoints
!*/
loadLibrary('zon:common.js');

/**
 * Wrapper for the xtk:persist#Write endpoint, which unencodes submitted data
 */
function zon_persist_WriteZip(input) {
  var queryXml = unzipAsXml(input, "WriteZip");
  xtk.session.Write(queryXml);
}

/**
 * Wrapper for the xtk:persist#WriteCollection endpoint, which unencodes submitted data
 */
function zon_persist_WriteCollectionZip(input) {
  var queryXml = unzipAsXml(input, "WriteCollectionZip");
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
  var folderId = getFolderIdByName(input["@folderName"]);
  if (!folderId.toString()) {
    return <response>
             <status>2</status>
             <message>Folder '{input["@folderName"]}' doesn't exist</message>
           </response>;
  }

  // TODO even if this works, it's not a good way to get the file extension.
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
  var existingFileResId = getFileResIdByName(input.fileRes["@internalName"]);
  if (existingFileResId > 0)
  {
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
  }
  else
  {
    // Create the file resource
    var fileResXML = <fileRes
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
    var fileRes = xtk.fileRes.create(fileResXML);
    fileRes.save();
    fileRes.PublishIfNeeded();
  }

  return <response>
           <status>0</status>
         </response>;
}
