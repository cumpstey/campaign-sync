using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zone.Campaign.Sync
{
    public static class EnumExtensions
    {
        public static string GetFileExtension(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = (FileExtensionAttribute[])field.GetCustomAttributes(typeof(FileExtensionAttribute), false);

            return (attributes.Any())
                ? attributes[0].FileExtension
                : value.ToString();
        }
    }
}
