using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Reflection;

/*
 * Custom implementation of this https://www.codeproject.com/articles/20917/creating-a-custom-settings-provider
 */
public class PortableSettingsProvider : SettingsProvider
{
	const string SETTINGS_ROOT = "settings";
	const string FILENAME = "config.settings";

	public override string ApplicationName
	{
		get
		{
			return Assembly.GetExecutingAssembly().GetName().Name;
		}
		set
		{
			//Do nothing
		}
	}

	public override string Name
	{
		get { return "PortableSettingsProvider"; }
	}

	public string Path
	{
		get
		{
			// %appdata%/Local/Twinder
			return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\"
				+ Assembly.GetExecutingAssembly().GetName().Name + "\\" + FILENAME;
		}
	}

	private XmlDocument _settingsXML;
	private XmlDocument SettingsXML
	{
		get
		{
			// If we dont hold an xml document, try opening one.  
			// If it doesnt exist then create a new one ready.
			if (_settingsXML == null)
			{
				_settingsXML = new XmlDocument();

				if (File.Exists(Path))
					_settingsXML.Load(Path);
				else
				{
					//Create new document
					XmlDeclaration dec = _settingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
					_settingsXML.AppendChild(dec);

					XmlNode nodeRoot = default(XmlNode);
					nodeRoot = _settingsXML.CreateNode(XmlNodeType.Element, SETTINGS_ROOT, string.Empty);
					_settingsXML.AppendChild(nodeRoot);
				}
			}

			return _settingsXML;
		}
	}


	public override void Initialize(string name, NameValueCollection collection)
	{
		base.Initialize(ApplicationName, collection);
	}
	
	public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propVals)
	{
		// Iterate through the settings to be stored
		// Only dirty settings are included in propvals, and only ones relevant to this provider
		foreach (SettingsPropertyValue propval in propVals)
		{
			SetValue(propval);
		}

		try
		{
			SettingsXML.Save(Path);
		}
		catch
		{
			//Ignore if cant save, device been ejected
		}
	}

	public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
	{
		//Create new collection of values
		SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

		//Iterate through the settings to be retrieved
		foreach (SettingsProperty setting in props)
		{
			SettingsPropertyValue value = new SettingsPropertyValue(setting);
			value.IsDirty = false;
			value.SerializedValue = GetValue(setting);
			values.Add(value);
		}
		return values;
	}

	private string GetValue(SettingsProperty setting)
	{
		string value = string.Empty;

		XmlNode node;
		if ((node = SettingsXML.SelectSingleNode(SETTINGS_ROOT + "/" + setting.Name)) != null)
			value = node.InnerText;
		else
		{
			if (setting.DefaultValue != null)
				value = setting.DefaultValue.ToString();
		}

		return value;
	}

	private void SetValue(SettingsPropertyValue propVal)
	{
		XmlNode node;
		// Check to see if the node exists, if so then set its new value
		if ((node = SettingsXML.SelectSingleNode(SETTINGS_ROOT + "/" + propVal.Name)) != null)
		{
			node.InnerText = propVal.SerializedValue.ToString();
		}
		else
		{
			node = SettingsXML.CreateElement(propVal.Name);
			node.InnerText = propVal.SerializedValue.ToString();
			SettingsXML.SelectSingleNode(SETTINGS_ROOT).AppendChild(node);
		}
	}
}