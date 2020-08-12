using System;
using System.Linq;
using System.Text;

[Serializable]
public static class StringBuilderExtension {
	#region Fields & Properties
	// ----------------------------------------------------------------------------------------------------
	private static readonly string[] UIntPairStrings = Enumerable.Range(0, 100).Select(n => n.ToString("00")).ToArray();
	private static readonly uint[] UIntPairScales = { 100000000, 1000000, 10000, 100 };
	// ----------------------------------------------------------------------------------------------------
	#endregion

	#region Extension Method
	// ----------------------------------------------------------------------------------------------------
	public static void AppendInvariant(this StringBuilder builder, uint value) {
		bool next = false;
		foreach (var scale in UIntPairScales) {
			if (value >= scale) {
				uint pair = value / scale;
				if (!next && pair < 10) {
					builder.Append((char)('0' + pair));
				}
				else {
					builder.Append(UIntPairStrings[pair]);
				}
				value -= pair * scale;
				next = true;
			}
			else if (next) {
				builder.Append("00");
			}
		}
		if (!next && value < 10) {
			builder.Append((char)('0' + value));
		}
		else {
			builder.Append(UIntPairStrings[value]);
		}
	}
	// ----------------------------------------------------------------------------------------------------
	#endregion
}
