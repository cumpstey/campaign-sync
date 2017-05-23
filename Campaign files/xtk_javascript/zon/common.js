/*!
Adobe Campaign metadata:
Schema: xtk:javascript
Name: zon:common.js
Label: Common functions
!*/
"use strict";

/**
 * @name NL.ZON
 * @namespace
 */
NL.ns("NL.ZON");
NL.require("/nl/core/shared/nl.js")
  .require("/nl/core/shared/xtk.js")
  .require("xtk:common.js");

(function(ns) {

/**
 * Deletes a file at the specified path if it exists.
 * @param {string} filePath - The path of the file to delete.
 */
var deleteFileIfExists = function(filePath) {
  var existingFile = new File(filePath);
  if (existingFile.exists) {
    existingFile.remove();
  }

  existingFile.dispose();
}

/**
 * Extracts content of a zip file, and parses it as xml. The zip should contain a single file called `queryName.xml`.
 * @param {string} content - Base64 encoded content of a zip file.
 * @param {string} queryName - The name of the xml file within the zip.
 */
var unzipAsXml = function(content, queryName) {

  // Global parameters
  var uploadDirPath = getOption("zon_UploadDirectory");

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

// // /**
// //  * Escapes any relevant characters so a string can be used in a query.
// //  */
// // function escapeForQuery(value) {
// //   return value.replace(/'/g, "\\'")
// //               .replace(/%/g, "\\%");
// // }

/**
 * Get the id of an xtk:folder with the specified name.
 * @param {string} name - The name of the folder.
 */
function getFolderIdByName(name) {
  var queryDef = DOMDocument.fromJXON({
    queryDef: {
      schema: "xtk:folder",
      operation: "getIfExists",
      select: {
        node: [
          { expr: "@id" },
        ]
      },
      where: {
        condition: [
          { expr: "@name = '" + NL.XTK.escape(name) + "'" },
        ]
      },
    }
  });
  var query = NLWS.xtkQueryDef.create(queryDef);
  var result = query.ExecuteQuery();
  return result ? result.$id : null;

  /*var query = new XML(
    <queryDef schema="xtk:folder" operation="getIfExists">
      <select><node expr="@id"/></select>
      <where><condition expr={"[@name] = '" + escapeForQuery(name) + "'"}/></where>
    </queryDef>);
  var queryResult = xtk.queryDef.create(query).ExecuteQuery();
  return queryResult.@id;*/
}

/**
 * Get the id of an xtk:fileRes with the specified name.
 * @param {string} name - The name of the fileRes.
 */
var getFileResIdByName = function(name) {
  var queryDef = DOMDocument.fromJXON({
    queryDef: {
      schema: "xtk:fileRes",
      operation: "getIfExists",
      select: {
        node: [
          { expr: "@id" },
        ]
      },
      where: {
        condition: [
          { expr: "@internalName = '" + NL.XTK.escape(name) + "'" },
        ]
      },
    }
  });
  var query = NLWS.xtkQueryDef.create(queryDef);
  var result = query.ExecuteQuery();
  return result ? result.$id : null;

  /*var query = new XML(
    <queryDef schema="xtk:fileRes" operation="getIfExists">
      <select><node expr="@id"/></select>
      <where><condition expr={"[@internalName] = '" + escapeForQuery(name) + "'"}/></where>
    </queryDef>);
  var queryResult = xtk.queryDef.create(query).ExecuteQuery();
  return queryResult.@id;*/
}

// Set namespace properties
ns.unzipAsXml = unzipAsXml;
ns.getFolderIdByName = getFolderIdByName;
ns.getFileResIdByName = getFileResIdByName;

}(NL.ZON.Publishing));
