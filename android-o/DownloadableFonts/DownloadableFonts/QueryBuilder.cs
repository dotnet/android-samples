using Java.Lang;

namespace DownloadableFonts
{
	public class QueryBuilder
	{
		String FamilyName = null;

		Float Width = null;

		Integer Weight = null;

		Float Italic = null;

		bool? Besteffort = null;

		public QueryBuilder(string familyName)
		{
			FamilyName = new String(familyName);
		}

		public QueryBuilder WithFamilyName(string familyName)
		{
			FamilyName = new String(familyName);
			return this;
		}

		public QueryBuilder WithWidth(float width)
		{
			if (width <= Constants.WIDTH_MIN)
			{
				throw new IllegalArgumentException("Width must be more than 0");
			}
			Width = new Float(width);
			return this;
		}

		public QueryBuilder WithWeight(int weight)
		{
			if (weight <= Constants.WEIGHT_MIN || weight >= Constants.WEIGHT_MAX)
			{
				throw new IllegalArgumentException(
						"Weight must be between 0 and 1000 (exclusive)");
			}
			Weight = new Integer(weight);
			return this;
		}

		public QueryBuilder WithItalic(float italic)
		{
			if (italic < Constants.ITALIC_MIN || italic > Constants.ITALIC_MAX)
			{
				throw new IllegalArgumentException("Italic must be between 0 and 1 (inclusive)");
			}
			Italic = new Float(italic);
			return this;
		}

		public QueryBuilder WithBestEffort(bool bestEffort)
		{
			Besteffort = bestEffort;
			return this;
		}

		public string Build()
		{
			if (Weight == null && Width == null && Italic == null && Besteffort == null)
			{
				return FamilyName.ToString();
			}
			StringBuilder builder = new StringBuilder();
			builder.Append("name=").Append(FamilyName);
			if (Weight != null)
			{
				builder.Append("&weight=").Append(Weight.ToString());
			}
			if (Width != null)
			{
				builder.Append("&width=").Append(Width.ToString());
			}
			if (Italic != null)
			{
				builder.Append("&italic=").Append(Italic.ToString());
			}
			if (Besteffort != null)
			{
				builder.Append("&besteffort=").Append(Besteffort.ToString());
			}
			return builder.ToString();
		}

	}
}
