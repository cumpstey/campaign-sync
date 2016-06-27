/*!
Adobe Campaign metadata:
Schema: xtk:javascript
Name: zon:queryDef.js
Label: Query def SOAP endpoints
!*/
loadLibrary('zon:common.js');

/**
 * Wrapper for the xtk:queryDef#ExecuteQuery endpoint, which unencodes submitted data
 */
function zon_queryDef_ExecuteQueryZip(input) {
  var queryXml = unzipAsXml(input, "ExecuteQuery");
  xtk.queryDef.ExecuteQuery(queryXml);
}
