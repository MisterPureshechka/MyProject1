using System.Collections.Generic;
using System.Text;
using Scripts.Meta;
using UnityEngine;

namespace Scripts.UI
{
    public static class StatTooltipBuilder
    {
        public static string BuildTooltip(MetaType parentType, Dictionary<string, Meta.Metadata> metadata)
        {
            var sb = new StringBuilder();

            sb.AppendLine(parentType.ToString());
            sb.AppendLine(new string('─', 13)); 

            foreach (var pair in metadata)
            {
                var meta = pair.Value;

                if (meta.MetaType != parentType)
                    continue;

                string name = meta.DisplayName ?? pair.Key;
                int value = Mathf.RoundToInt(meta.Value);

                sb.AppendLine($"{name,-8} {value}");
            }

            return sb.ToString();
        }
    }
}