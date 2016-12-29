function getPublishingInformation(fullFolderPath) {
  // Gets all the delivery and publishing information based on their folder path
  // It's necessary to make a direct db query, as it seems to be impossible to get
  // the publishing information back when querying for a delivery in the normal way.
  var cnx = application.getConnection();
  var result = cnx.query("SELECT del.iDeliveryId, del.sInternalName, del.sLabel, del.sPublishingName, del.sPublishingNamespace FROM NmsDelivery del INNER JOIN XtkFolder fol on fol.iFolderId = del.iFolderId  WHERE fol.sFullName LIKE $(sz)", fullFolderPath);
  var resultList = [];
  for(var row in result) {
    var resultObject = {
      id : row[0],
      internalName : row[1],
      label : row[2],
      publishingName : row[3],
      publishingNamespace : row[4]
    };
    resultList.push(resultObject);
  }
  cnx.dispose();
  return resultList;
}

var result = getPublishingInformation('/Content management/Delivery templates/Administration/%');



//logInfo(result.length);
//logInfo(JSON.stringify(result));
/*
for(var name in result[0])
{
  logInfo(name + " : " + result[0][name]);
}
*/

for (var i = 0; i < result.length; i++) {
  // Create a delivery object from the delivery Id which was taken from database
  // This object can be modified and saved back into the database
  var delivery = NLWS.nmsDelivery.load(result[i].id);

  // Get the publication name and namespace, needed for identifying a publication since they are files and are not located in database
  var publishingName = result[i].publishingName.toString().trim(); // Example: "newsletterBLN"
  var publishingNamespace = result[i].publishingNamespace.toString().trim(); // Example: "bar"

   // logInfo(JSON.stringify(result[i]));
   // logInfo(publishingNamespace + ':' + publishingName);

  if(!!publishingName && !!publishingNamespace)
  {
    logInfo('Transforming ' + delivery["@internalName"]);
    
    // Produce the publication file's id
    var publishingId = publishingNamespace + ':' + publishingName;
    // Get the delivery's content
    var xml = delivery.content.content.getFirstElement(publishingName);
    try {
      // Get the new delivery markup using the transform method
      var transformResult = ncm.publishing.Transform(publishingId, 'html', xml);
      // Overwrite the delivery content's html markup with the newly generated one and save it
      delivery.content.html.source = transformResult[1].toString();
      delivery.save();
    }
    catch(e) {
      logInfo(e);
    }
  }
  else
  {
    logInfo('Missing publishing information: ' + delivery["@internalName"]);
  }
}