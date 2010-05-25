using System.Collections.Generic;
using System.Text;

namespace EasyConfig
{
	/// <summary>
	/// A group of settings from a configuration file.
	/// </summary>
	public class SettingsGroup
	{
		/// <summary>
		/// Gets the name of the group.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the settings found in the group.
		/// </summary>
		public Dictionary<string, Setting> Settings { get; private set; }

		internal SettingsGroup(string name)
		{
			Name = name;
			Settings = new Dictionary<string, Setting>();
		}

		internal SettingsGroup(string name, List<Setting> settings)
		{
			Name = name;
			Settings = new Dictionary<string,Setting>();

			foreach (Setting setting in settings)
				Settings.Add(setting.Name, setting);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string name, int value)
		{
			addSetting(name, value.ToString(), false);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string name, float value)
		{
			addSetting(name, value.ToString(), false);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string name, bool value)
		{
			addSetting(name, value.ToString(), false);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string name, string value)
		{
			value = assertStringQuotes(value);
			addSetting(name, value, false);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The values of the setting.</param>
		public void AddSetting(string name, params int[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			addSetting(name, builder.ToString(), true);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The values of the setting.</param>
		public void AddSetting(string name, params float[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			addSetting(name, builder.ToString(), true);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The values of the setting.</param>
		public void AddSetting(string name, params bool[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i]);
				if (i < values.Length - 1)
					builder.Append(",");
			}

			addSetting(name, builder.ToString(), true);
		}

		/// <summary>
		/// Adds a setting to the group.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The values of the setting.</param>
		public void AddSetting(string name, params string[] values)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(assertStringQuotes(values[i]));
				if (i < values.Length - 1)
					builder.Append(",");
			}

			addSetting(name, builder.ToString(), true);
		}

		/// <summary>
		/// Deletes a setting from the group.
		/// </summary>
		/// <param name="name">The name of the setting to delete.</param>
		public void DeleteSetting(string name)
		{
			Settings.Remove(name);
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

		private void addSetting(string name, string value, bool isArray)
		{
			Settings.Add(name, new Setting(name, value, isArray));
		}
	}
}
