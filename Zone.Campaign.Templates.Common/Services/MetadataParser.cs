using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Zone.Campaign.Templates.Model;
using System.Collections.Generic;

namespace Zone.Campaign.Templates.Services
{
    public class MetadataParser : IMetadataFormatter, IMetadataParser
    {
        public string Format(TemplateMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            var items = new Dictionary<string, string> {
                { "Schema", metadata.Schema.ToString()},
                { "Name", metadata.Name.ToString() },
            };

            if (!string.IsNullOrEmpty(metadata.Label))
            {
                items.Add("Label", metadata.Label);
            }

            foreach (var item in metadata.AdditionalProperties.Where(i => !items.ContainsKey(i.Key)))
            {
                items.Add(item.Key, item.Value);
            }

            return string.Format("Adobe Campaign metadata:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, items.Select(i => string.Format("{0}: {1}", i.Key, i.Value))));
        }

        public TemplateMetadata Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var result = new TemplateMetadata();
            foreach (var metadatum in input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var metadatumMatch = Regex.Match(metadatum, @"^\s*(?<name>[a-z0-9_]+)\s*:\s*(?<value>.*?)\s*$", RegexOptions.IgnoreCase);
                if (!metadatumMatch.Success)
                {
                    continue;
                }

                var propertyName = metadatumMatch.Groups["name"].Value;
                var propertyValue = metadatumMatch.Groups["value"].Value;
                switch (propertyName.ToLower())
                {
                    case "schema":
                        {
                            InternalName value;
                            if (InternalName.TryParse(propertyValue, out value))
                            {
                                result.Schema = value;
                            }
                        }

                        break;
                    case "name":
                        {
                            InternalName value;
                            if (InternalName.TryParse(propertyValue, out value))
                            {
                                result.Name = value;
                            }
                        }

                        break;
                    case "label":
                        {
                            result.Label = propertyValue;
                        }

                        break;
                    default:
                        result.AdditionalProperties.Add(propertyName, propertyValue);
                        break;
                }
            }

            return result;
        }
    }
}
