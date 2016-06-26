/*!
Adobe Campaign metadata:
Schema: xtk:javascript
Name: zon:common.js
Label: Common functions
!*/
/**
 * Deletes a file at the specified path if it exists
 * @param {string} filePath - The path of the file to delete
 */
function deleteFileIfExists(filePath) {
  var existingFile = new File(filePath);
  if (existingFile.exists) {
    existingFile.remove();
  }
  
  existingFile.dispose();
}

/**
 * Extracts content of a zip file, and parses it as xml. The zip should contain a single file called `queryName.xml`
 * @param {string} content - Base64 encoded content of a zip file.
 * @param {string} queryName - The name of the xml file within the zip.
 */
function unzipAsXml(content, queryName) {
  var uploadDirPath = "C:\\Program Files (x86)\\Adobe\\Adobe Campaign v6\\var\\barrattv6_dev\\upload\\";
  
  var filePath = uploadDirPath + queryName + ".zip";

  // Delete existing file if it exists
  deleteFileIfExists(filePath);
  
  // Create file
  var buffer = new MemoryBuffer();
  buffer.appendBase64(content);
  buffer.save(filePath);
  buffer.dispose();

  // Extract query xml from zip
  var zip = new ZipFile(filePath);
  zip.open();
  var file = zip.getEntry(queryName + ".xml");
  var fileContent = file.toString();
  zip.dispose();

  // Clean up
  deleteFileIfExists(filePath);
 
  return fileContent; 
}
