﻿using System;
using System.Collections.Generic;
using TwainWeb.Standalone.Scanner;

namespace TwainWeb.Standalone.App
{
	public class CashSettings
	{
		private readonly List<ScannerSettings> _scannersSettings = new List<ScannerSettings>();
		private DateTime _lastUpdateTime = DateTime.UtcNow;

		public bool NeedUpdateNow(DateTime dateUtcNow)
		{
			return (dateUtcNow - _lastUpdateTime).Minutes >= 1;
		}

		public ScannerSettings Search(IScannerManager scannerManager, int searchIndex)
		{
			string sourceName;
			int? sourceId;
			try
			{
				var sourceProduct = scannerManager.GetSource(searchIndex);
				sourceName = sourceProduct.Name;
				sourceId = sourceProduct.Index;
			}
			catch (Exception)
			{
				return null;
			}

			foreach (var setting in _scannersSettings)
			{
				if (setting.Compare(sourceId.Value, sourceName))
					return setting;
			}
			return null;
		}

		/*public ScannerSettings Search(Twain32 _twain, int searchIndex)
		{
			string sourceName = null;
			int? sourceID = null;
			try
			{
				var sourceProduct = _twain.GetSourceProduct(searchIndex);
				sourceName = sourceProduct.Name;
				sourceID = sourceProduct.ID;
			}
			catch (Exception ex)
			{
				return null;
			}

			foreach (var setting in _scannersSettings)
			{
				if (setting.Compare(sourceID.Value, sourceName))
					return setting;
			}
			return null;
		}*/

		public void Update(IScannerManager scannerManager)
		{
			if (scannerManager.SourceCount > 0)
			{
				var settingsForDelete = new List<ScannerSettings>();
				
				foreach (var setting in _scannersSettings)
				{
					var activeSource = false;
					for (var i = 0; i < scannerManager.SourceCount; i++)
					{
						var sourceProduct = scannerManager.GetSource(i);
						if (setting.Compare(sourceProduct.Index, sourceProduct.Name))
						{
							activeSource = true;
							break;
						}
					}
					if (!activeSource)
						settingsForDelete.Add(setting);
				}
				foreach (var setting in settingsForDelete)
				{
					_scannersSettings.Remove(setting);
				}
			}
			else
				_scannersSettings.RemoveAll(x => true);

			_lastUpdateTime = DateTime.UtcNow;
		}

	/*	public void Update(Twain32 _twain)
		{
			if (_twain.CloseSM())
			{
				if (_twain.OpenSM())
				{
					if (_twain.SourcesCount > 0)
					{
						bool activeSource;
						var settingsForDelete = new List<ScannerSettings>();
						foreach (var setting in _scannersSettings)
						{
							activeSource = false;
							for (int i = 0; i < _twain.SourcesCount; i++)
							{
								var sourceProduct = _twain.GetSourceProduct(i);
								if (setting.Compare(sourceProduct.ID, sourceProduct.Name))
								{
									activeSource = true;
									break;
								}
							}
							if (!activeSource)
								settingsForDelete.Add(setting);
						}
						foreach (var setting in settingsForDelete)
						{
							this._scannersSettings.Remove(setting);
						}
					}
					else
						_scannersSettings.RemoveAll(x => true);

					_lastUpdateTime = DateTime.UtcNow;
				}
			}
		}*/

		public ScannerSettings PushCurrentSource(IScannerManager scannerManager)
		{
			var settings = scannerManager.CurrentSource.GetScannerSettings();
			_scannersSettings.Add(settings);
			return settings;
		}

		/*public ScannerSettings PushCurrentSource(Twain32 _twain)
		{
			Twain32.Enumeration resolutions = null;
			Twain32.Enumeration pixelTypes = null;
			float? maxHeight = null;
			float? maxWidth = null;
			try
			{
				resolutions = _twain.GetResolutions();
				pixelTypes = _twain.GetPixelTypes();
				maxHeight = _twain.GetPhisicalHeight();
				maxWidth = _twain.GetPhisicalWidth();
			}
			catch (Exception ex)
			{
			}

			//todo: конвертировать Twain32.Enumeration в List

			var sourceProduct = _twain.GetSourceProduct(_twain.SourceIndex);
/*            var settings = new ScannerSettings(sourceProduct.ID, sourceProduct.Name, resolutions.Items, pixelTypes, maxHeight, maxWidth);
            this.scannersSettings.Add(settings);
            return settings;#1#

			return null;
		}*/
	}
}