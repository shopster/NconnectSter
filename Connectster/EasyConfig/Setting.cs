using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Connectster.EasyConfig
{
	/// <summary>
	/// A single setting from a configuration file
	/// </summary>
	public class Setting
	{
		/// <summary>
		/// Gets the name of the setting.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the raw value of the setting.
		/// </summary>
		public string RawValue { get; private set; }

		/// <summary>
		/// Gets whether or not the setting is an array.
		/// </summary>
		public bool IsArray { get; private set; }

		internal Setting(string name, string value, bool isArray)
		{
			Name = name;
			RawValue = value;
			IsArray = isArray;
		}

		/// <summary>
		/// Attempts to return the setting's value as an integer.
		/// </summary>
		/// <returns>An integer representation of the value</returns>
		public int GetValueAsInt()
		{
			return int.Parse(RawValue, CultureInfo.InvariantCulture.NumberFormat);
		}

		/// <summary>
		/// Attempts to return the setting's value as a float.
		/// </summary>
		/// <returns>A float representation of the value</returns>
		public float GetValueAsFloat()
		{
			return float.Parse(RawValue, CultureInfo.InvariantCulture.NumberFormat);
		}

		/// <summary>
		/// Attempts to return the setting's value as a bool.
		/// </summary>
		/// <returns>A bool representation of the value</returns>
		public bool GetValueAsBool()
		{
			return bool.Parse(RawValue);
		}

		/// <summary>
		/// Attempts to return the setting's value as a string.
		/// </summary>
		/// <returns>A string representation of the value</returns>
		public string GetValueAsString()
		{
			if (!RawValue.StartsWith("\"") || !RawValue.EndsWith("\""))
				throw new Exception("Cannot convert value to string.");

			return RawValue.Substring(1, RawValue.Length - 2);
		}

		/// <summary>
		/// Attempts to return the setting's value as an array of integers.
		/// </summary>
		/// <returns>An integer array representation of the value</returns>
		public int[] GetValueAsIntArray()
		{
			string[] parts = RawValue.Split(',');

			int[] valueParts = new int[parts.Length];

			for (int i = 0; i < parts.Length; i++)
				valueParts[i] = int.Parse(parts[i], CultureInfo.InvariantCulture.NumberFormat);

			return valueParts;
		}

		/// <summary>
		/// Attempts to return the setting's value as an array of floats.
		/// </summary>
		/// <returns>An float array representation of the value</returns>
		public float[] GetValueAsFloatArray()
		{
			string[] parts = RawValue.Split(',');

			float[] valueParts = new float[parts.Length];

			for (int i = 0; i < parts.Length; i++)
				valueParts[i] = float.Parse(parts[i], CultureInfo.InvariantCulture.NumberFormat);

			return valueParts;
		}

		/// <summary>
		/// Attempts to return the setting's value as an array of bools.
		/// </summary>
		/// <returns>An bool array representation of the value</returns>
		public bool[] GetValueAsBoolArray()
		{
			string[] parts = RawValue.Split(',');

			bool[] valueParts = new bool[parts.Length];

			for (int i = 0; i < parts.Length; i++)
				valueParts[i] = bool.Parse(parts[i]);

			return valueParts;
		}

		/// <summary>
		/// Attempts to return the setting's value as an array of strings.
		/// </summary>
		/// <returns>An string array representation of the value</returns>
		public string[] GetValueAsStringArray()
		{
			Match match = Regex.Match(RawValue, "[\\\"][a-zA-Z\\d\\s]*[\\\"][,]*");

			List<string> values = new List<string>();

			while (match.Success)
			{
				string value = match.Value;
				if (value.EndsWith(","))
					value = value.Substring(0, value.Length - 1);

				value = value.Substring(1, value.Length - 2);
				values.Add(value);
				match = match.NextMatch();
			}

			return values.ToArray();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		public void SetValue(int value)
		{
			RawValue = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		public void SetValue(float value)
		{
			RawValue = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		public void SetValue(bool value)
		{
			RawValue = value.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		public void SetValue(string value)
		{
			RawValue = assertStringQuotes(value);
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new values to store.</param>
		public void SetValue(params int[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
				if (i < values.Length - 1)
					builder.Append(",");
			}

			RawValue = builder.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new values to store.</param>
		public void SetValue(params float[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
				if (i < values.Length - 1)
					builder.Append(",");
			}

			RawValue = builder.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new values to store.</param>
		public void SetValue(params bool[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			RawValue = builder.ToString();
		}

		/// <summary>
		/// Sets the value of the setting.
		/// </summary>
		/// <param name="value">The new values to store.</param>
		public void SetValue(params string[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(assertStringQuotes(values[i]));
				if (i < values.Length - 1)
					builder.Append(",");
			}

			RawValue = builder.ToString();
		}

		private static string assertStringQuotes(string value)
		{
			//make sure we have our surrounding quotations
			if (!value.StartsWith("\""))
				value = "\"" + value;
			if (!value.EndsWith("\""))
				value = value + "\"";
			return value;
		}
	}
}
