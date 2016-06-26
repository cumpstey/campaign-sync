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
function zon_persist_Write(input) {
  var queryXml = unzipAsXml(input, "Write");
  xtk.session.Write(queryXml);
}

/**
 * Wrapper for the xtk:persist#WriteCollection endpoint, which unencodes submitted data
 */
function zon_persist_WriteCollection(input) {
  var queryXml = unencodeAsXml(input);
  xtk.session.WriteCollection(queryXml);
}
