/*!
Adobe Campaign metadata:
Schema: xtk:javascript
Name: zon:publishing.js
Label: Publishing
!*/
"use strict";

loadLibrary("/nl/core/shared/nl.js");

NL.ns("NL.ZON.Publishing");
NL.require("/nl/core/shared/nl.js")
  .require("/nl/core/shared/xtk.js")
  .require("xtk:common.js");

/**
 * @name NL.ZON.Publishing
 * @namespace
 */
(function(ns) {

/**
 * Retrieves the names of the folders which are immediate children of the specified folder.
 * @param {string} folderName - Name of the folder of which to retrieve children.
 */
var getChildFolders = function(folderName) {
  var queryDef = DOMDocument.fromJXON({
    queryDef: {
      schema: "xtk:folder",
      operation: "select",
      select: {
        node: [
          { expr: "@name" },
        ]
      },
      where: {
        condition: [
          { expr: "[parent/@name] = '" + NL.XTK.escape(folderName) + "'" },
        ]
      },
    }
  });
  var query = NLWS.xtkQueryDef.create(queryDef);
  var results = query.ExecuteQuery();
  return results;
}

/**
 * Retrieves the names of delivery templates within the specified folder.
 * @param {string} folderName - Name of the folder of which to retrieve delivery templates.
 * @param {bool} recursive - Whether to return all delivery templates in descendent folders.
 */
var getDeliveryTemplates = function(folderName, recursive) {
  var queryDef = DOMDocument.fromJXON({
    queryDef: {
      schema: "nms:delivery",
      operation: "select",
      select: {
        node: [
          { expr: "@id" },
          { expr: "@internalName" },
          { expr: "@label" },
          { expr: "[@publishing-name]" },
          { expr: "[@publishing-namespace]" },
        ]
      },
      where: {
        condition: [
          { expr: "[folder/@name] = '" + NL.XTK.escape(folderName) + "'" },
          { expr: "@isModel = 1" },
        ]
      },
    }
  });
  var query = NLWS.xtkQueryDef.create(queryDef);
  var results = query.ExecuteQuery();

  if (recursive) {
    var subfolders = getChildFolders(folderName).getElements();
    for (var i = 0; i < subfolders.length; i++) {
      var subfolderResults = getDeliveryTemplates(subfolders[i].$name, true).getElements();
      for (var f = 0; f < subfolderResults.length; f++) {
        results.appendChild(results.ownerDocument.importNode(subfolderResults[f], true));
      }
    }
  }

  return results;
}

/**
 * Update markup for the specified delivery
 * @param {object} delivery - Delivery to republish.
 */
var republishContent = function(delivery) {

  // Get readable delivery name for logging
  var deliveryIdentifier = '"' + delivery.$label + '" (' + delivery.$internalName + ')';

  // Extract publishing namespace and name
  var publishingNamespace = delivery.getAttribute("publishing-namespace").toString().trim();
  var publishingName = delivery.getAttribute("publishing-name").toString().trim();

  // If no publishing details, reject delivery for publishing
  if (!publishingName || !publishingNamespace)
  {
    logWarning("Delivery " + deliveryIdentifier + " requested for publishing, but it doesn't have a publishing model.");
    return;
  }
  
  // Get the delivery in a form where we can update it
  var deliveryData = NLWS.nmsDelivery.load(delivery.$id.toString());

  // Get the delivery's content
  var content = deliveryData.content.content.getFirstElement(publishingName);
  
  try {
    // Get the new delivery markup using the transform method
    var htmlTransformResult = NLWS.ncmPublishing.Transform(publishingNamespace + ":" + publishingName, "html", content);
    var textTransformResult = ncm.publishing.Transform(publishingNamespace + ':' + publishingName, "text", content);

    // Overwrite the delivery's content with the newly generated content
    deliveryData.content.html.source = htmlTransformResult[1].toString();
    deliveryData.content.text.source = textTransformResult[1].toString();

    // Save the delivery
    deliveryData.save();

    logInfo("Delivery " + deliveryIdentifier + " republished.");
  } catch(e) {
    logWarning("Failed to republish delivery " + deliveryIdentifier + ": " + e);
  }
}

/**
 * Update markup for all delivery templates within the specified folder.
 * @param {string} folderName - Name of the folder containing the delivery templates.
 * @param {bool} recursive - Whether to update markup of all delivery templates in descendent folders.
 */
var updateMarkupForDeliveryTemplatesInFolder = function(folderName, recursive) {
  var deliveryTemplates = getDeliveryTemplates(folderName, recursive).getElements();
  for (var i = 0; i < deliveryTemplates.length; i++) {
    var deliveryTemplate = deliveryTemplates[i];
    
    republishContent(deliveryTemplate);
  }
}

// Set namespace properties
ns.updateMarkupForDeliveryTemplatesInFolder = updateMarkupForDeliveryTemplatesInFolder;

}(NL.ZON.Publishing));

